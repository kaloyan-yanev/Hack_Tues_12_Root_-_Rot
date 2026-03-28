using RootAndRot.Server.Data;
using RootAndRot.Server.Models;

namespace RootAndRot.Server.Services
{
    public interface IComposterService
    {
        public Task AddDevice(string MAC, string Username);
        public Task RemoveDevice(Guid DeviceId, string Username);
        public Task ChangeTempTreshold(Guid DeviceId, TempThresholdFactors factors);

       // public Task ChangeHumidityTreshold(ChangingHumidityTresholdDTO dto);

        public Task<IEnumerable<Device>> GetAllDataPerProfile(string Username);
        public Task StirTor(Guid DeviceId);
    }
}
