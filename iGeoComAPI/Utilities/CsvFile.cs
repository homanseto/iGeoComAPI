﻿using System.Text;
using System.IO;
using CsvHelper;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Utilities
{
    public static class CsvFile
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
