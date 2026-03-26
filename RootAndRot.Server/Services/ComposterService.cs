using Microsoft.EntityFrameworkCore.Metadata;
using RootAndRot.Server.Data;
using RootAndRot.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace RootAndRot.Server.Services
{
    public class ComposterService : IComposterService
    {
        private readonly AppDbContext _context;
        public ComposterService(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddDevice(string MAC, Guid Userid)
        {
            var user = await _context.Users
            .Include(u => u.Devices)
            .FirstOrDefaultAsync(x => x.UserId == Userid);

            // 2. Намираме устройството по неговия MAC адрес
            var device = await _context.Devices
                .FirstOrDefaultAsync(x => x.Macaddress == MAC);

            // 3. Проверка дали потребителят и устройството съществуват
            if (user == null || device == null)
            {
                throw new Exception("Потребителят или устройството не са намерени.");
            }

            // 4. Проверяваме дали устройството вече не е добавено към този потребител
            if (!user.Devices.Any(d => d.DeviceId == device.DeviceId))
            {
                user.Devices.Add(device);

                // EF Core автоматично разпознава промяната в Many-to-Many връзката,
                // затова _context.Users.Update(user) не е строго задължително тук.
                await _context.SaveChangesAsync();
            }
        }

        public async Task<float> ChangeTempTreshold(TempThresholdFactors factors)
        {
            return factors.CalculateTempThreshold();
        }

        public async Task<IEnumerable<Device>> GetAllDataPerProfile(Guid Userid)
        {
            return await _context.Users
             .Where(u => u.UserId == Userid)
             .SelectMany(u => u.Devices)
             .ToListAsync();
        }
    }
}
