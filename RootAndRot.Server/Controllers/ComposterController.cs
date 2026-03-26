using Microsoft.AspNetCore.Mvc;
using RootAndRot.Server.Models;
using RootAndRot.Server.Services;

namespace RootAndRot.Server.Controllers
{
    public class ComposterController : ControllerBase
    {
        private readonly IComposterService _composterService;
        public ComposterController(IComposterService composterService)
        {
            _composterService = composterService;
        }

        public async Task<IActionResult> ChangeTempTreshold(ChangingTempTresholdDTO dto)
        {
            TempThresholdFactors factors = new TempThresholdFactors()
            {
                placeholder1 = dto.placeholder1,
                placeholder2 = dto.placeholder2,
                placeholder3 = dto.placeholder3,
            };
            await _composterService.ChangeTempTreshold(factors);
            return Ok();
        }
    }
}
