using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Scripting.Hosting;
using RootAndRot.Server.Data;
using RootAndRot.Server.Models;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace RootAndRot.Server.Services
{
    public class ComposterService : IComposterService
    {
        private readonly AppDbContext _context;
        private readonly ScriptEngine _engine;
        private readonly string _scriptsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PythonScripts");
        public ComposterService(AppDbContext context)
        {
            _context = context;
            _engine = Python.CreateEngine();
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

        public async Task ChangeTempTreshold(Guid DeviceId, TempThresholdFactors factors)
        {
            Device device = await _context.Devices
                .FirstOrDefaultAsync(x => x.DeviceId == DeviceId);

            RunScriptFile("send_tresh.py", device.Macaddress, factors.CalculateTempThreshold());
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
                throw new FileNotFoundException($"Скриптът не е намерен: {fullPath}");

            // 1. Подготвяме аргументите (първият винаги е името на скрипта)
            var argv = new List<object>{fileName, mac, value};

            // 2. Подаваме аргументите към общата Python среда
            _engine.GetSysModule().SetVariable("argv", argv);

            // 3. Изпълняваме файла
            ScriptScope scope = _engine.CreateScope();
            _engine.ExecuteFile(fullPath, scope);
        }
    }
}
