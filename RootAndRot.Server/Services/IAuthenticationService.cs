using RootAndRot.Server.Models;

namespace RootAndRot.Server.Services
{
    public interface IAuthenticationService
    {
        public Task LogIn(AuthenticationDTO dto);
    }
}
