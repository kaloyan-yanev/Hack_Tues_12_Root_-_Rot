using Microsoft.AspNetCore.Mvc;
using RootAndRot.Server.Services;

namespace RootAndRot.Server.Controllers
{
    public class LoggingController : ControllerBase
    {
        private readonly IAuthenticationService _logginservice;
    }
}
