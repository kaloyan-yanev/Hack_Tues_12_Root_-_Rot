using RootAndRot.Server.Data;
using RootAndRot.Server.Models;

namespace RootAndRot.Server.Services
{
    public interface IComposterService
    {
        public Task AddDevice(string MAC, Guid Userid);
        public Task ChangeTempTreshold(Guid DeviceId, TempThresholdFactors factors);

       // public Task ChangeHumidityTreshold(ChangingHumidityTresholdDTO dto);

        public Task<IEnumerable<Device>> GetAllDataPerProfile(Guid Userid);
        public Task StirTor(Guid DeviceId);
    }
}
