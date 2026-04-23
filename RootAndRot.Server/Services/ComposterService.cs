using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RootAndRot.Server.Data;
using RootAndRot.Server.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RootAndRot.Server.Services
{
    public class ComposterService : IComposterService
    {
        private readonly AppDbContext _context;
        private readonly string _scriptsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PythonScripts");
        public ComposterService(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddDevice(string MAC, string Username)
        {
            var user = await _context.Users
            .Include(u => u.Devices)
            .FirstOrDefaultAsync(x => x.Name == Username);

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

        public async Task RemoveDevice(Guid DeviceId, string Username)
        {
            var user = await _context.Users
                .Include(u => u.Devices)
                .FirstOrDefaultAsync(x => x.Name == Username);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var device = user.Devices.FirstOrDefault(d => d.DeviceId == DeviceId);
            if (device == null)
            {
                throw new Exception("Device not found for this user.");
            }

            user.Devices.Remove(device);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeTempThreshold(Guid DeviceId, bool hasVegetables, bool hasMeat, bool hasDairy)
        {
            var device = await _context.Devices
               .FirstOrDefaultAsync(x => x.DeviceId == DeviceId);

            if (device == null)
            {
                throw new Exception("Device not found.");
            }

            device.DoesntHaveMeatOrDairy = hasVegetables;
            device.HasMeat = hasMeat;
            device.HasDairy = hasDairy;
            _context.Devices.Update(device);
            await _context.SaveChangesAsync();

            float number;
            if (hasDairy)
            {
                number = 70.0f;
            }
            else if (hasMeat)
            {
                number = 60.0f;
            }
            else if (hasVegetables)
            {
                number = 50.0f;
            }
            else
            {
                number = 40.0f;
            }
            RunScriptFile("send_tresh.py", device.Macaddress, number);
        }

        public async Task<IEnumerable<Device>> GetAllDataPerProfile(string Username)
        {
            return await _context.Users
             .Where(u => u.Name == Username)
             .SelectMany(u => u.Devices)
             .ToListAsync();
        }
        public async Task StirTor(Guid DeviceId)
        {
            Device device = await _context.Devices
                .FirstOrDefaultAsync(x => x.DeviceId == DeviceId);
            if (device == null)
            {
                throw new Exception("Device not found.");
            }

            RunScriptFile("send_stir.py", device.Macaddress, 1);

        }
        private void RunScriptFile(string fileName, string mac, object value)
        {
            string fullPath = Path.Combine(_scriptsPath, fileName);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Script not found: {fullPath}");

            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{fullPath}\" {mac} {value}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            process!.WaitForExit(TimeSpan.FromSeconds(15));

            if (process.ExitCode != 0)
            {
                string error = process.StandardError.ReadToEnd();
                throw new Exception($"Python script '{fileName}' failed (exit {process.ExitCode}): {error}");
            }
        }
    }
}
