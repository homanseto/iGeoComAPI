using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;

namespace iGeoComAPI
{
    public class MyBackGroundService : BackgroundService
    {
        private string InsertSql = "INSERT INTO igeocomtable VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID,@Latitude,@Longitude);";
        private readonly ILogger<MyBackGroundService> _logger;
        private readonly DataAccess _dataAccess;
        private IGrabberAPI<SevenElevenModel> _sevenElevenGrabber;
        private readonly WellcomeGrabber _wellcomeGrabber;
        private readonly USelectGrabber _uSelectGrabber;
        private IGrabberAPI<ParknShopModel> _parknShopGrabber;

        public MyBackGroundService(ILogger<MyBackGroundService> logger, DataAccess dataAccess, IGrabberAPI<SevenElevenModel> sevenElevenGrabber, WellcomeGrabber wellcomeGrabber, USelectGrabber uSelectGrabber, IGrabberAPI<ParknShopModel> parknShopGrabber)
        {
            _logger = logger;
            _dataAccess = dataAccess;
            _wellcomeGrabber = wellcomeGrabber;
            _sevenElevenGrabber = sevenElevenGrabber;
            _uSelectGrabber = uSelectGrabber;
            _parknShopGrabber = parknShopGrabber;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
             _logger.LogInformation("From MyBackGroundService: ExecuteGrabbingAsync {dateTime}", DateTime.Now);
             var sevenElevenResult = await _sevenElevenGrabber.GetWebSiteItems();
             _dataAccess.SaveGrabbedData(InsertSql, sevenElevenResult);
             var wellcomeResult = await _wellcomeGrabber.GetWebSiteItems();
             _dataAccess.SaveGrabbedData(InsertSql, wellcomeResult);
             var uSelectResult = await _uSelectGrabber.GetWebSiteItems();
             _dataAccess.SaveGrabbedData(InsertSql, uSelectResult);
             var parkNShopResult = await _parknShopGrabber.GetWebSiteItems();
              _dataAccess.SaveGrabbedData(InsertSql, parkNShopResult);
             await Task.Delay(TimeSpan.FromHours(10), stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("From MyBackGroundService: StopAsync {dateTime}", DateTime.Now);
            return base.StopAsync(stoppingToken);
        }


    }
}
