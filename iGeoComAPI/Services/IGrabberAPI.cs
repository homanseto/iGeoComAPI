using iGeoComAPI.Models;

namespace iGeoComAPI.Services
{
    public interface IGrabberAPI<T>
    {
        Task<List<IGeoComModel>?> GetWebSiteItems();
        List<IGeoComModel> MergeEnAndZh(List<T> enResult, List<T> zhResult);
    }
}