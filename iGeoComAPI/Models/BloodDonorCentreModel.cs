namespace iGeoComAPI.Models
{
    public class BloodDonorCentreModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address  { get; set; } = string.Empty;
        public string LatLng { get; set; } = string.Empty;
        public static string ReplaceExtraInfo 
        {
            get {return @"(^.+LatLng\(|\),.*location_map_pop_title fs_20"">|<a class=""marker_detail_link fs_12"".*<div class=""location_addr"">|<\/div><div class=""location_openhour"">.*)"; }
        }  
        public static string RegLagLngRegex
        {
            get { return "[^,]*"; }
        }
        public static string ExtraList
        {
            get { return @"\};var locations = \[(.*)];var map"; }
        } 
        public static string ReplaceExtraInfoWithWorkingHour { 
        get { return @"(^.+LatLng\(|\),.*location_map_pop_title fs_20"">|<a class=""marker_detail_link fs_12"".*<div class=""location_addr"">|<\/div><div class=""location_openhour"">|<\/div><div class=""location_addtocalander"">.*)";
            }
        }
    }
}
