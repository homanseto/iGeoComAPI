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
builder.Services.AddSingleton<IGrabberAPI<CaltexModel>, CaltexGrabber>();
builder.Services.AddSingleton<IGrabberAPI<ParknShopModel>, ParknShopGrabber>();
builder.Services.AddSingleton<IGrabberAPI<AeonModel>, AeonGrabber>();
builder.Services.AddSingleton<IGrabberAPI<AromeNMaximsCakesModel>,AromeNMaximsCakesGrabber>();
builder.Services.AddSingleton<AmbulanceDepotGrabber>();
builder.Services.AddSingleton<WellcomeGrabber>();
builder.Services.AddSingleton<CircleKGrabber>();
builder.Services.AddSingleton<VangoGrabber>();
builder.Services.AddSingleton<USelectGrabber>();
builder.Services.AddSingleton<WmoovGrabber>();
builder.Services.AddSingleton<ConnectClient>();
builder.Services.AddSingleton<JsonFunction>();
builder.Services.AddSingleton<DataAccess>();
builder.Services.AddSingleton<PuppeteerConnection>();
builder.Services.AddMemoryCache();
builder.Services.Configure<ConnectionStringsHomeOptions>(_configuration.GetSection(ConnectionStringsHomeOptions.SectionName));
builder.Services.Configure<ConnectionStrings3DMOptions>(_configuration.GetSection(ConnectionStrings3DMOptions.SectionName));
builder.Services.Configure<SevenElevenOptions>(_configuration.GetSection(SevenElevenOptions.SectionName));
builder.Services.Configure<WellcomeOptions>(_configuration.GetSection(WellcomeOptions.SectionName));
builder.Services.Configure<CaltexOptions>(_configuration.GetSection(CaltexOptions.SectionName));
builder.Services.Configure<ParknShopOptions>(_configuration.GetSection(ParknShopOptions.SectionName));
builder.Services.Configure<AeonOptions>(_configuration.GetSection(AeonOptions.SectionName));
builder.Services.Configure<VangoOptions>(_configuration.GetSection(VangoOptions.SectionName));
builder.Services.Configure<USelectOptions>(_configuration.GetSection(USelectOptions.SectionName));
builder.Services.Configure<CircleKOptions>(_configuration.GetSection(CircleKOptions.SectionName));
builder.Services.Configure<WmoovOptions>(_configuration.GetSection(WmoovOptions.SectionName));
builder.Services.Configure<AmbulanceDepotOptions>(_configuration.GetSection(AmbulanceDepotOptions.SectionName));
builder.Services.Configure<AromeNMaximsCakesOptions>(_configuration.GetSection(AromeNMaximsCakesOptions.SectionName));
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
