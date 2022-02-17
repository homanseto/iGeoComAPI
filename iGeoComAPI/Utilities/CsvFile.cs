using System.Text;
using System.IO;

namespace iGeoComAPI.Utilities
{
    public static class CsvFile
    {
        public static void DownloadCsv<T>(List<T> shopList, string fileName)
        {
            
            DateTime now = DateTime.Now;
            string nowName = now.ToString().Replace(@"/", "").Replace(@":","_").Replace(" ","");
            var sb = new StringBuilder();
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var finalPath = Path.Combine(basePath, $"Downloads\\{fileName}{nowName}.csv");
            string header = "";
            var info = typeof(T).GetProperties();


            /*
            if (!File.Exists(finalPath))
            {
                var file = File.Create(finalPath);
                file.Close();
                header = string.Join(",", info.Select(p => p.Name));
                var dataLines = from shop in shopList
                                let shopLine = string.Join(",", shop.GetType().GetProperties().
                                Select(p => p.GetValue(shop)))
                                select shopLine;
                var csvData = new List<string>();
                csvData.Add(header);
                csvData.AddRange(dataLines);

                File.WriteAllLines(finalPath, csvData);
            }
            */

            
                if (!File.Exists(finalPath))
            {
                var file = File.Create(finalPath);
                file.Close();
                
                foreach (var prop in typeof(T).GetProperties())
                {
                    header += prop.Name + ", ";
                }
                header = header.Substring(0, header.Length - 2);
                sb.AppendLine(header);
                StreamWriter sw = new StreamWriter(finalPath, true, Encoding.UTF8);
                sw.Write(sb.ToString());
                sw.Close();
            }
            foreach (var obj in shopList)
            {
                sb = new StringBuilder();
                var line = "";
                foreach (var prop in info)
                {
                    line += prop.GetValue(obj, null) + ",";
                }
                line = line.Substring(0, line.Length - 2);
                sb.AppendLine(line);
                TextWriter sw = new StreamWriter(finalPath, true, Encoding.UTF8);
                sw.Write(sb.ToString());
                sw.Close();
            }
            
        }
    }
}
