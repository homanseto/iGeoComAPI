
namespace iGeoComAPI.Utilities
{
    public interface IDataAccess
    {
        Task DeleteDataFromDataBase<T>(string sql);
        Task<List<T>> LoadData<T>(string sql, object param);
        void SaveGrabbedData<T>(string sql, List<T> parameters);
    }
}