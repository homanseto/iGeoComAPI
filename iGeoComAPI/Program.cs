using iGeoComAPI.Services;
using iGeoComAPI.Options;
using Microsoft.Extensions.DependencyInjection;
using iGeoComAPI.Utilities;
using Serilog;
using iGeoComAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigurationManager _configuration = builder.Configuration;
IWebHostEnvironment _environment = builder.Environment;
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(_configuration)
    .Enrich.WithThreadId()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddHttpClient();
builder.Services.AddSingleton<IGrabberAPI<SevenElevenModel>,SevenElevenGrabber>();
builder.Services.AddSingleton< IGrabberAPI < WellcomeModel>, WellcomeGrabber >();
builder.Services.AddSingleton<IGrabberAPI<WellcomeModel>, WellcomeGrabber>();
builder.Services.AddSingleton<IGrabberAPI<CaltexModel>, CaltexGrabber>();
builder.Services.AddSingleton<IGrabberAPI<ParknShopModel>, ParknShopGrabber>();
builder.Services.AddSingleton<WmoovGrabber>();
builder.Services.AddSingleton<ConnectClient>();
builder.Services.AddSingleton<SerializeFunction>();
builder.Services.AddSingleton<DataAccess>();
builder.Services.AddSingleton<PuppeteerConnection>();
builder.Services.AddMemoryCache();
builder.Services.Configure<SevenElevenOptions>(_configuration.GetSection(SevenElevenOptions.SectionName));
builder.Services.Configure<ConnectionStringsOptions>(_configuration.GetSection(ConnectionStringsOptions.SectionName));
builder.Services.Configure<WellcomeOptions>(_configuration.GetSection(WellcomeOptions.SectionName));
builder.Services.Configure<CaltexOptions>(_configuration.GetSection(CaltexOptions.SectionName));
builder.Services.Configure<ParknShopOptions>(_configuration.GetSection(ParknShopOptions.SectionName));
builder.Services.Configure<WmoovOptions>(_configuration.GetSection(WmoovOptions.SectionName));
builder.Services.AddOptions(); //IOptions<T>


var app = builder.Build();
app.Logger.LogInformation("Project start");
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
