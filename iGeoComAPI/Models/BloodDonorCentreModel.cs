namespace iGeoComAPI.Models
{
    public class BloodDonorCentreModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Address  { get; set; }
        public string? LatLng { get; set; }
        public string SelectCaltexFromDataBase
        {
            get { return "SELECT * FROM iGeoCom_Dec2021 WHERE ENGLISHNAME like '%Caltex%';"; }
        } 
        public string SelectCaltex
        {
            get { return "SELECT * FROM igeocomtable WHERE GRAB_ID LIKE '%caltex%'"; }
        }
        public string ReplaceExtraInfo 
        {
            get {return @"(^.+LatLng\(|\),.*location_map_pop_title fs_20"">|<a class=""marker_detail_link fs_12"".*<div class=""location_addr"">|<\/div><div class=""location_openhour"">.*)"; }
        }  
        public string RegLagLngRegex
        {
            get { return "([^,]*)"; }
        }
        public string ExtraList
        {
            get { return @"\};var locations = \[(.*)];var map"; }
        } 
        public string ReplaceExtraInfoWithWorkingHour { 
        get { return @"(^.+LatLng\(|\),.*location_map_pop_title fs_20"">|<a class=""marker_detail_link fs_12"".*<div class=""location_addr"">|<\/div><div class=""location_openhour"">|<\/div><div class=""location_addtocalander"">.*)";
            }
        } 
    }
}
