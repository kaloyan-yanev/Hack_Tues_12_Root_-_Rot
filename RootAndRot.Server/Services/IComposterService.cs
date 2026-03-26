using RootAndRot.Server.Data;
using RootAndRot.Server.Models;

namespace RootAndRot.Server.Services
{
    public interface IComposterService
    {
        public Task AddDevice(string MAC);
        public Task<float> ChangeTempTreshold(TempThresholdFactors factors);

       // public Task ChangeHumidityTreshold(ChangingHumidityTresholdDTO dto);

        public Task<IEnumerable<Device>> GetAllDataPerProfile(Guid id);
    }
}
