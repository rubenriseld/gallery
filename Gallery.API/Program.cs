using AutoMapper;
using Gallery.API.DTOs;
using Gallery.API.Endpoints;
using Gallery.API.Interfaces;
using Gallery.API.Services;
using Gallery.Database;
using Gallery.Database.Entities;
using Gallery.Database.Interfaces;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapImageEndpoints();
app.MapTagEndpoints();
app.MapImageCollectionEndpoints();

app.UseHttpsRedirection();

app.Run();