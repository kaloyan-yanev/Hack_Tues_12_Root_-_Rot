using Microsoft.AspNetCore.Mvc;
using RootAndRot.Server.Models;
using RootAndRot.Server.Services;

namespace RootAndRot.Server.Controllers
{
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }
        public async Task<IActionResult> Register(RegistrationDTO dto)
        {
            string name = dto.Name;
            string password = dto.Password;
            await _registrationService.RegisterUser(name,password);
            return Ok();
        }
    }
}
