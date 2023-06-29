using AutoMapper;
using Gallery.Common.DTOs;
using Gallery.Database.Contexts;
using Gallery.Database.Entities;
using Gallery.Database.Interfaces;
using Gallery.Database.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<GalleryContext>(
	options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));


builder.Services.AddCors(policy =>
{
	policy.AddPolicy("CorsAllAccessPolicy", opt =>
		opt.AllowAnyOrigin()
		.AllowAnyHeader()
		.AllowAnyMethod()
	);
});

ConfigureAutoMapper(builder.Services);

builder.Services.AddScoped<IGalleryService, GalleryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


void ConfigureAutoMapper(IServiceCollection services)
{
	var config = new MapperConfiguration(cfg =>
	{
		cfg.CreateMap<Image, ImageDTO>().ReverseMap();
		cfg.CreateMap<ImageCreateDTO, Image>();
		cfg.CreateMap<ImageEditDTO, Image>();
		cfg.CreateMap<Image, ImageBaseDTO>();

		cfg.CreateMap<ImageCollection, ImageCollectionDTO>().ReverseMap();
		cfg.CreateMap<ImageCollectionCreateDTO, ImageCollection>();
		cfg.CreateMap<ImageCollectionEditDTO, ImageCollection>();
	});
	var mapper = config.CreateMapper();
	services.AddSingleton(mapper);
}