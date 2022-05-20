
namespace iGeoComAPI.Utilities
{
    public interface IDataAccess
    {
        Task DeleteDataFromDataBase<T>(string sql);
        Task<List<T>> LoadData<T>(string sql, object param);
        Task<T> LoadSingleData<T>(string sql);
        void SaveGrabbedData<T>(string sql, List<T> parameters);
    }
}