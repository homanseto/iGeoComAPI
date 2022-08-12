using iGeoComAPI.Controllers;
using Serilog;

namespace iGeoComAPI.Utilities
{
    
    public class MyLogger 
    {
        private readonly ILogger<MyLogger> _logger;
        public MyLogger(ILogger<MyLogger> logger )
        {
            _logger = logger;
        }


        public void LogControllerRequest(string controller, string action )
        {
            _logger.LogInformation("Request received by Controller: {Controller}, Action: {ControllerAction}", nameof(controller), action );
        }

        public void LogStartGrabbing(string grabber)
        {
            _logger.LogInformation("start grabber:{grabberName}", grabber);
        }

        public void LogMergeEngAndZh(string model)
        {
            _logger.LogInformation("Start merging {Model} eng and Zh", model);
        }

    }
}
