using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SelfCheckout.API.Configurations;
using SelfCheckout.API.Extensions;
using SelfCheckout.API.Interfaces;
using SelfCheckout.API.Middlewares;
using SelfCheckout.API.Services;
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
builder.Services.AddApiVersioning(setup =>
{
    setup.DefaultApiVersion = new ApiVersion(1, 0);
    setup.AssumeDefaultVersionWhenUnspecified = true;
    setup.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddApiDoc();

// Register configurations
builder.Services.Configure<MoneysConfiguration>(builder.Configuration.GetSection(nameof(MoneysConfiguration)));

// Register databases
builder.Services.AddDbContext<SelfCheckoutDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SelfCheckoutDb")));

// Register services
builder.Services.AddScoped<IStockService, StockService>();

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

app.UseSwagger();
app.UseApiDocs();

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
