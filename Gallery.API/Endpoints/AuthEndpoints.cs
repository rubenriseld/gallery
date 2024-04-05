using Gallery.API.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Gallery.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/auth");

        group.MapPost("/logout", Logout).RequireAuthorization();

        group.MapPost("/login", Login);

        //ONLY USED FOR CREATING ACCOUNT IN TESTING
        //group.MapPost("/register", Register);

    }
    public static async Task<IResult> Register(UserManager<IdentityUser> userManager, LoginDTO login)
    {
        var user = new IdentityUser(login.Email);
        user.Email = login.Email;
        await userManager.CreateAsync(user, login.Password);

        return Results.Created();
    }
    public static async Task<IResult> Login(SignInManager<IdentityUser> signInManager, LoginDTO login)
    {
        signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;
        await signInManager.PasswordSignInAsync(login.Email, login.Password, true, lockoutOnFailure: true);
        return Results.Ok();
    }
    public static async Task<IResult> Logout(SignInManager<IdentityUser> signInManager)
    {
        await signInManager.SignOutAsync().ConfigureAwait(false);
        return Results.NoContent();
    }
}