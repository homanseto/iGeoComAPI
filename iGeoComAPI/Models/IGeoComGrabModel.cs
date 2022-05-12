using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace iGeoComAPI.Models
{

   
    public class IGeoComGrabModel :IGeoComModel
    {
        public string GrabId { get; set; } =String.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
