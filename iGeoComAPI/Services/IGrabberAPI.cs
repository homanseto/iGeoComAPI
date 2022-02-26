using iGeoComAPI.Models;

namespace iGeoComAPI.Services
{
    public interface IGrabberAPI<T>
    {
        Task<List<IGeoComGrabModel>?> GetWebSiteItems();
        List<IGeoComGrabModel> MergeEnAndZh(List<T> enResult, List<T> zhResult);
    }
}