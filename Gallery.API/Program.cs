using AutoMapper;
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
services.AddScoped<IRepository<ImageTag>, Repository<ImageTag>>();

services.AddScoped<IImageService, ImageService>();
services.AddScoped<ITagService, TagService>();

services.AddSingleton(new MapperConfiguration(cfg =>
{
    cfg.CreateMap<Image, ReadImageDTO>()
        .ForMember(
            dest => dest.ImageCollectionName,
            opt => opt.MapFrom(src => src.ImageCollection != null ? src.ImageCollection.Name : string.Empty)
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

//Identity services
services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies()
    .ApplicationCookie!.Configure(opt => opt.Events = new CookieAuthenticationEvents()
    {
        OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
    });

services.AddAuthorizationBuilder();

services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<GalleryDbContext>()
    .AddApiEndpoints();

//Connection
services.AddDbContext<GalleryDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("GalleryDbContext"));
});

var app = builder.Build();

app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync().ConfigureAwait(false);
}).RequireAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//TODO implement custom login endpoint or filter out unused Identity endpoints
app.MapIdentityApi<IdentityUser>();
app.MapImageEndpoints();
app.MapTagEndpoints();

app.UseHttpsRedirection();

app.Run();