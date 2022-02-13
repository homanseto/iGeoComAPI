﻿using iGeoComAPI.Models;
using iGeoComAPI.Services;
using Microsoft.AspNetCore.Mvc;
using iGeoComAPI.Options;
using Microsoft.Extensions.Options;
using iGeoComAPI.Utilities;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SevenElevenController : ControllerBase

    {
       // private readonly ConfigurationManager _configuration;
        private readonly ILogger<SevenElevenController> _logger;
        private readonly SevenElevenGrabber _sevenElevenGrabber;
        private readonly DataAccess _dataAccess;
        private readonly IOptions<DataSQLOptions> _options;
        public SevenElevenController(SevenElevenGrabber sevenElevenGrabber, ILogger<SevenElevenController> logger, DataAccess dataAccess, IOptions<DataSQLOptions> options)
        {
            _sevenElevenGrabber = sevenElevenGrabber;
            _logger = logger;
            _options = options;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComModel>?> Get()
        {
            var result = await _sevenElevenGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(_options.Value.InsertSql, result);
            return result;
        }

        
    }

}
