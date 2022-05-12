using HKMap.Entities;

namespace HKMap.Contracts
{
    public interface IMapRepository
    {
        public Task<HKRegion> GetRegion(double latitude, double longitude);
    }
}
