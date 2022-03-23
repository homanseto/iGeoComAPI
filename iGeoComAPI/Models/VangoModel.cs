﻿namespace iGeoComAPI.Models
{
    public class VangoModel
    {
        public string? address_geo_lat { get; set; }
        public string? address_geo_lng { get; set; }
        public string? store_number { get; set; }
        public string? storename { get; set; }
        public string? address_description { get; set; }
        public string? telephone { get; set; }
        public string? telephone2 { get; set; }
        public string? telephone3   { get; set; }
        public string SelectVangoFromDataBase
        {
            get { return "SELECT * FROM iGeoCom_Dec2021 WHERE ENGLISHNAME like '%vango%'"; }
        } 
        public string SelectVango
        {
            get { return "SELECT * FROM igeocomtable WHERE GRAB_ID LIKE '%vango%'"; }
        } 
    }
}
