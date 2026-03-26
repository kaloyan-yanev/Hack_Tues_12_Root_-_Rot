using RootAndRot.Server.Models;

namespace RootAndRot.Server.Services
{
    public interface IRegistrationService
    {
        public Task RegisterUser(RegistrationDTO dto);
    }
}
