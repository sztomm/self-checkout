using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SelfCheckout.API.Configurations;
using SelfCheckout.API.Extensions;
using SelfCheckout.API.Middlewares;
using SelfCheckout.DAL;
using Serilog;
using System;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostContextBuilder, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(builder.Configuration, sectionName: "Serilog"));
builder.Configuration.AddJsonFile("moneys.json", false, false);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MoneysConfiguration>(builder.Configuration.GetSection(nameof(MoneysConfiguration)));

builder.Services.AddDbContext<SelfCheckoutDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SelfCheckoutDb")));

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger>();
logger.Information($"");
logger.Information($"API Starting... Environment is set to: '{app.Environment.EnvironmentName}'; API version: {Assembly.GetEntryAssembly().GetName().Version}");
logger.Information($"");

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<SelfCheckoutDbContext>();
if (dbContext == null) throw new ArgumentException("Db context is null!", nameof(dbContext));
dbContext.MigrateDb(logger);
dbContext.SeedData(logger, scope.ServiceProvider.GetService<IOptions<MoneysConfiguration>>());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
