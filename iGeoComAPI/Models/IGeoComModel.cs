using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Models
{
    public class IGeoComModel
    {
        public string GeoNameId { get; set; } = String.Empty;
        public string EnglishName { get; set; } = String.Empty;
        public string ChineseName { get; set; } = String.Empty;
        public string Class { get; set; } = String.Empty;
        public string Type { get; set; } = String.Empty;
        public string Subcat { get; set; } = String.Empty;
        public double Easting { get; set; }
        public double Northing { get; set; }
        public string Source { get; set; } = String.Empty;
        public string E_floor { get; set; } = String.Empty;
        public string C_floor { get; set; } = String.Empty;
        public string E_sitename { get; set; } = String.Empty;
        public string C_sitename { get; set; } = String.Empty;
        public string E_area { get; set; } = String.Empty;
        public string C_area { get; set; } = String.Empty;
        public string E_District { get; set; } = String.Empty;
        public string C_District { get; set; } = String.Empty;
        public string E_Region { get; set; } = String.Empty;
        public string C_Region { get; set; } = String.Empty;
        public string E_Address { get; set; } = String.Empty;
        public string C_Address { get; set; } = String.Empty;
        public string Tel_No { get; set; } = String.Empty;
        public string Fax_No { get; set; } = String.Empty;
        public string Web_Site { get; set; } = String.Empty;
        public DateTime Rev_Date { get; set; } = DateTime.Now;
    }

}
