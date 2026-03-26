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
            await _registrationService.RegisterUser(dto);
            return Ok();
        }
    }
}
