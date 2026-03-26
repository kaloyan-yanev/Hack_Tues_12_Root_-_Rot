using RootAndRot.Server.Data;
using RootAndRot.Server.Models;

namespace RootAndRot.Server.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppDbContext _appDbContext;
        public AuthenticationService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public Task LogIn(AuthenticationDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
