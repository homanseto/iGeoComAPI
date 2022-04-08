using iGeoComAPI.Services;
using iGeoComAPI.Options;
//using Microsoft.Extensions.DependencyInjection;
using iGeoComAPI.Utilities;
using Serilog;
using iGeoComAPI.Models;
using iGeoComAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigurationManager _configuration = builder.Configuration;
IWebHostEnvironment _environment = builder.Environment;
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(_configuration)
    .Enrich.WithThreadId()
    .Enrich.FromLogContext()
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
builder.Services.AddSingleton<BloodDonorCentreGrabber>();
builder.Services.AddSingleton<WellcomeGrabber>();
builder.Services.AddSingleton<CircleKGrabber>();
builder.Services.AddSingleton<VangoGrabber>();
builder.Services.AddSingleton<USelectGrabber>();
builder.Services.AddSingleton<WmoovGrabber>();
builder.Services.AddSingleton<BmcpcGrabber>();
builder.Services.AddSingleton<CSLGrabber>();
builder.Services.AddSingleton<CheungKongGrabber>();
builder.Services.AddSingleton<CatholicOrgGrabber>();
builder.Services.AddSingleton<ChinaMobileGrabber>();
builder.Services.AddSingleton<ConnectClient>();
builder.Services.AddSingleton<JsonFunction>();
builder.Services.AddSingleton<DataAccess>();
builder.Services.AddSingleton<PuppeteerConnection>();
builder.Services.AddSingleton<MyLogger>();
builder.Services.AddMemoryCache();
builder.Services.Configure<AppSettingOptions>(a => new AppSettingOptions { Environment = _environment.EnvironmentName });
MyConfigServiceCollection.AddConfig(builder.Services, _configuration);
builder.Services.AddOptions(); //IOptions<T>

if(_environment.EnvironmentName == "Production")
{
    builder.Services.AddHostedService<MyBackGroundService>();
}
var app = builder.Build();
app.Logger.LogInformation("Project start");
// Configure the HTTP request pipeline.
if (_environment.EnvironmentName == "Development" | _environment.EnvironmentName == "Development_Home")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
