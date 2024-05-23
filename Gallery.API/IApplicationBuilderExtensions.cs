using Microsoft.AspNetCore.Identity;

namespace Gallery.API;

public static class IApplicationBuilderExtensions
{
    public static IApplicationBuilder CreateAdminAccount(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var adminEmail = configuration["AdminCredentials:Email"] ?? throw new ArgumentNullException($"Could not get {nameof(IdentityUser.Email)} from the {configuration["AdminCredentials"]}.");

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            if (bool.TryParse(configuration["DatabaseConfiguration:SeedAdminAccount"], out _) && userManager.FindByEmailAsync(adminEmail).Result == null)
            {
                var hasher = new PasswordHasher<IdentityUser>();
                var adminUser = new IdentityUser
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    PasswordHash = hasher.HashPassword(null!, configuration["AdminCredentials:Password"] ?? throw new ArgumentNullException($"Could not get {nameof(IdentityUser.PasswordHash)} from the {configuration["AdminCredentials"]}.")),
                };

                IdentityResult userCreated = userManager.CreateAsync(adminUser).Result;
                if (!userCreated.Succeeded)
                {
                    throw new ApplicationException("Something went wrong when trying to create the admin configuration.");
                }
            }
        }
        return app;
    }   
}
