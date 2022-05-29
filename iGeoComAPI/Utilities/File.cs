using System.Text;
using System.IO;
using CsvHelper;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using iGeoComAPI.Models;

namespace iGeoComAPI.Utilities
{
    public static class File
    {
        public static FileStreamResult Download<T>(List<T> shopList, string fileName)
        {
            string now = DateTime.Now.ToString().Replace(@"/", "").Replace(@":", "").Replace(" ", "");
            MemoryStream ms;
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords<T>(shopList);
                streamWriter.Flush();
                var result = memoryStream.ToArray();
                ms = new MemoryStream(result);
            }
            return new FileStreamResult(ms, "text/csv") { FileDownloadName = $"{fileName}_{now}.csv" };
        }

        public static FileStreamResult DownloadDelta(List<IGeoComDeltaModel> shopList, string fileName)
        {
            string now = DateTime.Now.ToString().Replace(@"/", "").Replace(@":", "").Replace(" ", "");
            List<DeltaModel> newList = new List<DeltaModel>();
            foreach (var delta in shopList)
            {
                DeltaModel newShop = new DeltaModel();
                newShop.status = delta.status;
                newShop.GeoNameId = delta.GeoNameId;
                newShop.EnglishName = delta.EnglishName;
                newShop.ChineseName = delta.ChineseName;
                newShop.Class = delta.Class;
                newShop.Type = delta.Type;
                newShop.Subcat = delta.Subcat;
                newShop.Easting = delta.Easting;
                newShop.Northing = delta.Northing;
                newShop.Source = delta.Source;
                newShop.E_floor = delta.E_floor;
                newShop.C_floor = delta.C_floor;
                newShop.E_sitename = delta.E_sitename;
                newShop.C_sitename = delta.C_sitename;
                newShop.E_area = delta.E_area;
                newShop.C_area = delta.C_area;
                newShop.C_District = delta.C_District;
                newShop.E_District = delta.E_District;
                newShop.E_Region = delta.E_Region;
                newShop.C_Region = delta.C_Region;
                newShop.E_Address = delta.E_Address;
                newShop.C_Address = delta.C_Address;
                newShop.Tel_No = delta.Tel_No;
                newShop.Fax_No = delta.Fax_No;
                newShop.Web_Site = delta.Web_Site;
                newShop.Rev_Date = delta.Rev_Date;
                newList.Add(newShop);
            }
            MemoryStream ms;
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
             
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(newList);
                streamWriter.Flush();
                var result = memoryStream.ToArray();
                ms = new MemoryStream(result);
            }
            return new FileStreamResult(ms, "text/csv") { FileDownloadName = $"{fileName}_{now}.csv" };
        }


        //public static FileInfo DownloadExcel<T>(List<T> shopList, string fileName)
        //{
        //    string now = DateTime.Now.ToString().Replace(@"/", "").Replace(@":", "").Replace(" ", "");
        //    MemoryStream ms;
        //    using (var memoryStream = new MemoryStream())
        //    using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
        //    using (ExcelPackage excel = new ExcelPackage())
        //    {
        //        var sheet1 = excel.Workbook.Worksheets.Add("WorkSheet1");
        //        sheet1.Cells.LoadFromCollection(shopList, true);

        //    }
        //    return new FileStreamResult(ms, "text/csv") { FileDownloadName = $"{fileName}_{now}.csv" };
        //}

        //public static void DownloadCsv<T>(List<T> shopList, string fileName)
        //{
        //    DateTime now = DateTime.Now;
        //    string nowName = now.ToString().Replace(@"/", "").Replace(@":", "_").Replace(" ", "");
        //    var basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        //    var finalPath = Path.Combine(basePath, $"Downloads\\{fileName}{nowName}.csv");
        //    using (var writer = new StreamWriter(finalPath, true, Encoding.UTF8))
        //    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //    {
        //        csv.WriteRecords(shopList);
        //    }

        //    /*
        //    DateTime now = DateTime.Now;
        //    string nowName = now.ToString().Replace(@"/", "").Replace(@":","_").Replace(" ","");
        //    var sb = new StringBuilder();
        //    var basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        //    var finalPath = Path.Combine(basePath, $"Downloads\\{fileName}{nowName}.csv");
        //    string header = "";
        //    var info = typeof(T).GetProperties();

        //        if (!File.Exists(finalPath))
        //    {
        //        var file = File.Create(finalPath);
        //        file.Close();

        //        foreach (var prop in typeof(T).GetProperties())
        //        {
        //            header += prop.Name + ", ";
        //        }
        //        header = header.Substring(0, header.Length - 2);
        //        sb.AppendLine(header);
        //        StreamWriter sw = new StreamWriter(finalPath, true, Encoding.UTF8);
        //        sw.Write(sb.ToString());
        //        sw.Close();
        //    }
        //    foreach (var obj in shopList)
        //    {
        //        sb = new StringBuilder();
        //        var line = "";
        //        foreach (var prop in info)
        //        {
        //            line += prop.GetValue(obj, null) + ",";
        //        }
        //        line = line.Substring(0, line.Length - 2);
        //        sb.AppendLine(line);
        //        TextWriter sw = new StreamWriter(finalPath, true, Encoding.UTF8);
        //        sw.Write(sb.ToString());
        //        sw.Close();
        //    }
        //    */

        //}
    }
}
