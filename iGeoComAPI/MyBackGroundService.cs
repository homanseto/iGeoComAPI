using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using System.Diagnostics;

namespace iGeoComAPI
{
    public class MyBackGroundService : BackgroundService
    {
        private readonly ILogger<MyBackGroundService> _logger;
        private readonly DataAccess _dataAccess;
        private IGrabberAPI<SevenElevenModel> _sevenElevenGrabber;
        private readonly WellcomeGrabber _wellcomeGrabber;
        private readonly USelectGrabber _uSelectGrabber;
        private IGrabberAPI<ParknShopModel> _parknShopGrabber;

        IGeoComModel igeoComModel = new IGeoComModel();

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
             Stopwatch timer = new Stopwatch();
                timer.Start();
             _logger.LogInformation("From MyBackGroundService: ExecuteGrabbingAsync {dateTime}", DateTime.Now);
             var sevenElevenResult = await _sevenElevenGrabber.GetWebSiteItems();
             _dataAccess.SaveGrabbedData(igeoComModel.InsertSql, sevenElevenResult);
             var wellcomeResult = await _wellcomeGrabber.GetWebSiteItems();
             _dataAccess.SaveGrabbedData(igeoComModel.InsertSql, wellcomeResult);
             var uSelectResult = await _uSelectGrabber.GetWebSiteItems();
             _dataAccess.SaveGrabbedData(igeoComModel.InsertSql, uSelectResult);
             var parkNShopResult = await _parknShopGrabber.GetWebSiteItems();
              _dataAccess.SaveGrabbedData(igeoComModel.InsertSql, parkNShopResult);
                timer.Stop();
             var timeTaken = timer.Elapsed.TotalHours;
             await Task.Delay(TimeSpan.FromHours(timeTaken +24), stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("From MyBackGroundService: StopAsync {dateTime}", DateTime.Now);
            return base.StopAsync(stoppingToken);
        }


    }
}
