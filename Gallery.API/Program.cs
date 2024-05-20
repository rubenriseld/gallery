using AutoMapper;
using Gallery.API;
using Gallery.API.DTOs;
using Gallery.API.Endpoints;
using Gallery.API.Interfaces;
using Gallery.API.Services;
using Gallery.Database;
using Gallery.Database.Entities;
using Gallery.Database.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

//Database services
services.AddScoped<IRepository<Image>, Repository<Image>>();
services.AddScoped<IRepository<ImageCollection>, Repository<ImageCollection>>();
services.AddScoped<IRepository<Tag>, Repository<Tag>>();

services.AddScoped<IImageService, ImageService>();
services.AddScoped<ITagService, TagService>();
services.AddScoped<IImageCollectionService, ImageCollectionService>();

services.AddSingleton(new MapperConfiguration(cfg =>
{
    cfg.CreateMap<Image, ReadImageDTO>()
        .ForMember(
            dest => dest.ImageCollectionName,
            opt => opt.MapFrom(src => src.ImageCollection.Name)
        );

    cfg.CreateMap<CreateImageDTO, Image>();
    cfg.CreateMap<UpdateImageDTO, Image>();

    cfg.CreateMap<ImageCollection, ReadImageCollectionDTO>();
    cfg.CreateMap<CreateImageCollectionDTO, ImageCollection>();
    cfg.CreateMap<UpdateImageCollectionDTO, ImageCollection>();

    cfg.CreateMap<Tag, ReadTagDTO>();
    cfg.CreateMap<CreateTagDTO, Tag>();
    cfg.CreateMap<UpdateTagDTO, Tag>();

}).CreateMapper());

//Swagger/OpenAPI services
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins("https://rubenriseld.se")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
})
.AddCookie(IdentityConstants.ApplicationScheme, options =>
{
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.HttpOnly = true; // Configure HttpOnly
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Configure Secure policy to Always
    options.ExpireTimeSpan = TimeSpan.FromDays(150); // Expire cookie after 150 days
    options.Events = new CookieAuthenticationEvents()
    {
        OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
    };
});
services.AddAuthorization();

services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<GalleryDbContext>()
    .AddApiEndpoints();

services.AddDbContext<GalleryDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("GalleryDbContext"));
});

services.AddExceptionHandler<ExceptionHandler>();
services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler(_ => { });

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.CreateAdminAccount();

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});
app.UseAuthentication();
app.UseAuthorization();

app.MapImageEndpoints();
app.MapTagEndpoints();
app.MapImageCollectionEndpoints();
app.MapAuthEndpoints();

app.Run();