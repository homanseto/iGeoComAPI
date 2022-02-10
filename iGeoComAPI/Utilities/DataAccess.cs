using Dapper;
using iGeoComAPI.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;

namespace iGeoComAPI.Utilities
{
    public class DataAccess
    {
        public async Task<List<IGeoComModel>> LoadData(string sql, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                var rows = await connection.QueryAsync<IGeoComModel>(sql);

                return rows.ToList();
            }
        }

        public void SaveGrabbedData(string sql, List<IGeoComModel> parameters, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                foreach(var param in parameters)
                {
                    connection.Execute(sql, param);
                }
            }
        }

        /*
        public void SaveGrabbedData(string sql, List<IGeoComModel> parameters, string connectionString)
        {
            StringBuilder sCommand = new StringBuilder(sql);
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                List<string> Rows = new List<string>();
                foreach (var row in parameters.Select((value, i) => new { i, value }))
                {
                    //ENGLISHNAME,CHINESENAME,E_DISTRICT, C_DISTRICT, E_REGION, C_REGION, E_ADDRESS, C_ADDRESS,latitude, longitude
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                        MySqlHelper.EscapeString(row.value.Subcat),
                        MySqlHelper.EscapeString(row.value.E_District),
                        MySqlHelper.EscapeString(row.value.C_District),
                        MySqlHelper.EscapeString(row.value.E_Region),
                        MySqlHelper.EscapeString(row.value.C_Region),
                        MySqlHelper.EscapeString(row.value.E_Address),
                        MySqlHelper.EscapeString(row.value.C_Address),
                        MySqlHelper.EscapeString(row.value.Latitude),
                        MySqlHelper.EscapeString(row.value.Longitude),
                        MySqlHelper.EscapeString(row.value.GrabID)
                        ));
                }
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");
                connection.Open();
                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), (MySqlConnection)connection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }

                //return connection.ExecuteAsync(sql, parameters);

            }
        }
        */
    }
}
