using Gallery.Database;
using Gallery.Database.Entities;
using Gallery.Database.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddScoped<IRepository<Image>, Repository<Image>>();
services.AddScoped<IRepository<ImageCollection>, Repository<ImageCollection>>();
services.AddScoped<IRepository<Tag>, Repository<Tag>>();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddDbContext<GalleryDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("GalleryDbContext"));
});

services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<GalleryDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
