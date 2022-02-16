using iGeoComAPI.Services;
using iGeoComAPI.Options;
using Microsoft.Extensions.DependencyInjection;
using iGeoComAPI.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigurationManager _configuration = builder.Configuration;
IWebHostEnvironment _environment = builder.Environment;

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddHttpClient();
builder.Services.AddSingleton<SevenElevenGrabber>();
builder.Services.AddSingleton<WellcomeGrabber>();
builder.Services.AddSingleton<ConnectClient>();
builder.Services.AddSingleton<SerializeFunction>();
builder.Services.AddSingleton<DataAccess>();
builder.Services.AddSingleton<PuppeteerConnection>();
builder.Services.AddMemoryCache();
builder.Services.Configure<SevenElevenOptions>(_configuration.GetSection(SevenElevenOptions.SectionName));
builder.Services.Configure<ConnectionStringsOptions>(_configuration.GetSection(ConnectionStringsOptions.SectionName));
builder.Services.Configure<DataSQLOptions>(_configuration.GetSection(DataSQLOptions.SectionName));
builder.Services.Configure<WellcomeOptions>(_configuration.GetSection(WellcomeOptions.SectionName));
builder.Services.AddOptions(); //IOptions<T>


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
