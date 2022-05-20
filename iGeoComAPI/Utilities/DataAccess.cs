using Dapper;
using iGeoComAPI.Models;
using iGeoComAPI.Options;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace iGeoComAPI.Utilities
{
    public class DataAccess: IDataAccess
    {
        private readonly IOptions<ConnectionStringsOptions> _options;
        private readonly IOptions<AppSettingOptions> _env;

        public DataAccess(IOptions<ConnectionStringsOptions> options, IOptions<AppSettingOptions> env)
        {
            _options = options;
            _env = env;
        }

        public async Task<List<T>> LoadData<T>(string sql, object param )
        {

            if (_env.Value.Environment == "Development" || _env.Value.Environment == "Production")
            {
                using (SqlConnection connection = new SqlConnection(_options.Value.Default_3DM))
                {
                    var rows = await connection.QueryAsync<T>(sql, param);

                    return rows.ToList();
                }
            }
            else
            {

                using (SqlConnection connection = new SqlConnection(_options.Value.DefaultConnection))
                {
                    var rows = await connection.QueryAsync<T>(sql, param);

                    return rows.ToList();
                }
            }

        }

        public async Task<T> LoadSingleData<T>(string sql)
        {

             using (SqlConnection connection = new SqlConnection(_options.Value.Default_3DM))
                {
                    var info = await connection.QuerySingleOrDefaultAsync<T>(sql);

                    return info;
                }

        }

        public void SaveGrabbedData<T>(string sql, List<T> parameters)
        {

            if (_env.Value.Environment == "Development" || _env.Value.Environment == "Production")
            {
                using (SqlConnection connection = new SqlConnection(_options.Value.Default_3DM))
                {
                    foreach (var param in parameters)
                    {
                        connection.Execute(sql, param);
                    }
                }
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(_options.Value.DefaultConnection))
                {
                    foreach (var param in parameters)
                    {
                        connection.Execute(sql, param);
                    }
                }
            }
        }

        public async Task DeleteDataFromDataBase<T>(string sql)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.DefaultConnection))
            {
                await connection.QueryAsync<T>(sql);
            }
        }


        /*
List<T> output;
output = _memoryCache.Get<List<T>>("iGeoCom");

if(output is null)
{
    // get result fro memory

    //how much and how long you save data in cache
    _memoryCache.Set("iGeoCom", output, TimeSpan.FromMinutes(1));

}
return output;
*/


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
