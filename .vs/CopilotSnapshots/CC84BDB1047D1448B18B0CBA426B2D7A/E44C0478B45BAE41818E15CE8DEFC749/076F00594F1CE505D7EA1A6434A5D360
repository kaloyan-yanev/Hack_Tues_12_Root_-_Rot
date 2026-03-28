using RootAndRot.Server.Data;
using RootAndRot.Server.Models;

namespace RootAndRot.Server.Services
{
    public interface IComposterService
    {
        public Task AddDevice(string MAC, string Username);
        public Task RemoveDevice(Guid DeviceId, string Username);
        public Task ChangeTempThreshold(Guid DeviceId, bool hasVegetables, bool hasMeat, bool hasDairy);
        public Task<IEnumerable<Device>> GetAllDataPerProfile(string Username);
        public Task StirTor(Guid DeviceId);
    }
}
