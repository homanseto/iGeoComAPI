using iGeoComAPI.Entities;

namespace iGeoComAPI.Models
{
    public interface IIGeoComModel
    {
        string C_Address { get; set; }
        string C_area { get; set; }
        string C_District { get; set; }
        string C_floor { get; set; }
        string C_Region { get; set; }
        string C_sitename { get; set; }
        string ChineseName { get; set; }
        string Class { get; set; }
        string E_Address { get; set; }
        string E_area { get; set; }
        string E_District { get; set; }
        string E_floor { get; set; }
        string E_Region { get; set; }
        string E_sitename { get; set; }
        double Easting { get; set; }
        string EnglishName { get; set; }
        string Fax_No { get; set; }
        string GeoNameId { get; set; }
        double Northing { get; set; }
        DateTime Rev_Date { get; set; }
        string Source { get; set; }
        string Subcat { get; set; }
        string Tel_No { get; set; }
        string Type { get; set; }
        string Web_Site { get; set; }

        Task<List<IGeoComEntity>> GetShops(string keyword);
        Task<List<IGeoComEntity>> GetShopsByType(string keyword);
    }
}