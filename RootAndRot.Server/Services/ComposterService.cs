using Microsoft.EntityFrameworkCore.Metadata;
using RootAndRot.Server.Data;
using RootAndRot.Server.Models;

namespace RootAndRot.Server.Services
{
    public class ComposterService : IComposterService
    {
        private readonly AppDbContext _context;
        public ComposterService(AppDbContext context)
        {
            _context = context;
        }
        public Task AddDevice(string MAC)
        {
            throw new NotImplementedException();
        }

        public async Task<float> ChangeTempTreshold(TempThresholdFactors factors)
        {
            return factors.CalculateTempThreshold();
        }

        public Task<IEnumerable<Device>> GetAllDataPerProfile(Guid Userid)
        {
            throw new NotImplementedException();
        }
    }
}
