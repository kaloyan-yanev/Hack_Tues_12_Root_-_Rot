using RootAndRot.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace RootAndRot.Server.Services
{
    public interface IRegistrationService
    {
        public Task RegisterUser(string name, string password);
    }
}
