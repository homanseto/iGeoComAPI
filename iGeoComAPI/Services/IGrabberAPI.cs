using iGeoComAPI.Models;

namespace iGeoComAPI.Services
{
    public interface IGrabberAPI
    {
        Task<List<IGeoComModel>?> GetWebSiteItems();
        List<IGeoComModel> MergeEnAndZh(List<SevenElevenModel> enResult, List<SevenElevenModel> zhResult);
        Task SaveDataBase();
    }
}