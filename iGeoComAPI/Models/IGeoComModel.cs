using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace iGeoComAPI.Models
{
    public class IGeoComModel
    {
        //public enum Types
        //{
        //    GHS,HCV,HTL,YHL, YAC, CEM, COL, CRE, FUN, ACS, CTR, CVS, FLR, MAL, ROI, SMK, CMC, FSC, RCM, VOF, CHL, DMT, EXB, HST, LIB, PAG, PFM, SCU, SSR, TNC,TTH,ABL,CMS,CSD,CST,DLO,DOF,FSN,GOD,GOF,
        //    ICA,ICP,IDB,IDM,IDP,IDR,JUD,LJB,LRE,LRY,MAP,ORG,POB,POF,PPT,PSI,PSN,RPT,TAN,TDT,BDC,CLI,ELD,HOS,REH,BAZ,CFS,CSC,MKT,RCP,TOI,WMT,CPB,CRA,CSA,MPB,MRA,NRA,SPA,CHU,MON,MOS,RPS,SMY,SYN,TMP,AMK,
        //    BAS,BWG,BWR,CCS,GCO,IGH,ISR,OSA,PAR,PAV,PLG,RCO,RGD,SCO,SGD,SPL,STD,TCT,VPT,ZOO,CCC,KDG,MIS,PRS,SCM,SEC,SES,,TEI,VTI,SIG,AIR,CBC,CBV,CPO,DTC,FER,FET,HLP,HSN,HSA,LRA,MTA,PIE,PTA,RSN,TOA,EES,
        //    LPG,PFS,SGN,SST 
        //}
        public string GeoNameId { get; set; } =String.Empty;
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
        public int Shop { get; set; }
        public DateTime Rev_Date { get; set; } = DateTime.Now;

        public string Compare_E_Address
        {
            get
            {
                return this.E_Address.ToLower().Replace(" ", "").Replace(",", "").Replace(".", "").Replace("/", "").Replace("(", "").Replace(")", "").Replace("，", "").Replace("<br/>","").Replace("<br />","").Replace("\t", "").Replace("\\", "").Replace("。", "").Trim();
            }
        }

        public string Compare_C_Address
        {
            get
            {
                return this.C_Address.ToLower().Replace(" ", "").Replace(",", "").Replace(".", "").Replace("/", "").Replace("(", "").Replace(")", "").Replace("，","").Replace("<br/>", "").Replace("<br />", "").Replace("\t", "").Replace("\\", "").Replace("。", "").Trim();
            }
        }
        public string Compare_ChineseName
        {
            get
            {
                return this.ChineseName.ToLower().Replace(" ", "").Replace(",", "").Replace("(", "").Replace(")", "").Replace("<br/>", "").Replace("\t", "").Replace("\\", "").Replace("。", "").Trim();
            }
        }
        public string Compare_EnglishName
        {
            get
            {
                return this.EnglishName.ToLower().Replace(" ", "").Replace(",", "").Replace("(", "").Replace(")", "").Replace("<br/>", "").Replace("\t", "").Replace("\\", "").Replace("。", "").Trim();
            }
        }
        public string Compare_Tel
        {
            get
            {
                return this.Tel_No.Replace(" ", "").Replace("-", "").Replace("\t", "").Replace("+", "").Replace("(", "").Replace("<br/>", "").Replace(")", "").Replace("\\", "").Replace("。", "").Trim();
            }
        }
    }

}
