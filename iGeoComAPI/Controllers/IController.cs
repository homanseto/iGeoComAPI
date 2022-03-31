using iGeoComAPI.Models;

namespace iGeoComAPI.Controllers
{
    public interface IController
    {
        Task<List<IGeoComGrabModel?>> Get();
        Task<List<IGeoComGrabModel>?> Post();
    }
}