using Firmaro.API.Utils;
using Firmaro.Application.DTOs;
using Firmaro.Application.Interfaces.Services;
using Firmaro.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmaro.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AutomationSettingsController : ControllerBase
    {
        private readonly IAutomationSettingsService _service;

        public AutomationSettingsController(IAutomationSettingsService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            AutomationSettings settings = await _service.GetSettingsAsync(User.GetUserId());
            return Ok(settings);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAutomationSettingsRequest request)
        {
            await _service.UpdateSettingsAsync(User.GetUserId(), request);
            return NoContent();
        }
    }
}