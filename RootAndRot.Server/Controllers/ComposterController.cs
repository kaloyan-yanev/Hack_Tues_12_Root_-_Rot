using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RootAndRot.Server.Data;
using RootAndRot.Server.Models;
using RootAndRot.Server.Services;
using System.Security.Claims;

namespace RootAndRot.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ComposterController : ControllerBase
    {
        private readonly IComposterService _composterService;
        public ComposterController(IComposterService composterService)
        {
            _composterService = composterService;
        }
        [HttpPost("ChangeTempThreshold")]
        public async Task<IActionResult> ChangeTempTreshold(ChangingTempTresholdDTO dto)
        {
            Guid DeviceId = dto.DeviceId;
            TempThresholdFactors factors = new TempThresholdFactors()
            {
                placeholder1 = dto.placeholder1,
                placeholder2 = dto.placeholder2,
                placeholder3 = dto.placeholder3,
            };
            await _composterService.ChangeTempTreshold(DeviceId, factors);
            return Ok();
        }
        [HttpGet("GetAllData")]
        public async Task<IActionResult> GetAllData()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest("Invalid user identity.");

            var devices = (await _composterService.GetAllDataPerProfile(userId))
                .Select(DeviceDataDTO.FromDevice)
                .ToList();

            return Ok(devices);
        }
        [HttpPost("AddDevice")]
        public async Task<IActionResult> AddDevice([FromBody] AddingDeviceDTO dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(!Guid.TryParse(userIdClaim, out var userId))
            {
                return BadRequest("Invalid user identity");
            }
            await _composterService.AddDevice(dto.MACAddress, userId);
            return Ok();
        }
        public async Task<IActionResult> StirTor()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return BadRequest("Invalid user identity");
            }
            await _composterService.StirTor(userId);
            return Ok();
        }
    }
}
