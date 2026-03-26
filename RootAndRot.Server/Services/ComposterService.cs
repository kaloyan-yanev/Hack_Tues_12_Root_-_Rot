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

        public async Task ChangeTempTreshold(TempThresholdFactors factors)
        {
            await factors.CalculateTempThreshold();
        }

        public Task<IEnumerable<Device>> GetAllDataPerDevice(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
