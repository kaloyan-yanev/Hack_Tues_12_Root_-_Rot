using RootAndRot.Server.Data;
using RootAndRot.Server.Models;

namespace RootAndRot.Server.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly AppDbContext _context;
        public RegistrationService(AppDbContext context)
        {
            _context = context;
        }
        public async Task RegisterUser(string name, string password)
        {
            User user = new User()
            {
                Name = name,
                Password = password
            };
            _context.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
