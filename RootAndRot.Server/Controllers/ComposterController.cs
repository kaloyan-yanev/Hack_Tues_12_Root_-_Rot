using Microsoft.AspNetCore.Mvc;
using RootAndRot.Server.Data;
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
        [HttpPost]
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
        [HttpGet]
        public async Task<IActionResult> GetAllData(Guid Userid)
        {
            var devices = (await _composterService.GetAllDataPerProfile(Userid)).Select(DeviceDataDTO.FromDevice).ToList();
            return Ok(devices);
        }
        [HttpPost]
        public async Task<IActionResult> AddDevice(string MAC)
        {
            await _composterService.AddDevice(MAC);
            return Ok();
        }
    }
}
