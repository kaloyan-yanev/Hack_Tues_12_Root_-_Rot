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
        public async Task RegisterUser(RegistrationDTO dto)
        {
            User user = new User();
            user.Name = dto.Name;
            user.Password = dto.Password;
            _context.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
