using Firmaro.API.Utils;
using Firmaro.Application.DTOs;
using Firmaro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmaro.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _service;
        public AppointmentController(IAppointmentService service)
        {
            _service = service;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
        {
            try
            {
                AppointmentResponse response = await _service.CreateAsync(User.GetUserId(), request);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (ArgumentException ex) 
            { 
                return BadRequest(new { error = ex.Message }); 
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync(User.GetUserId()));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try 
            { 
                return Ok(await _service.GetByIdAsync(id, User.GetUserId()));
            }
            catch (KeyNotFoundException) 
            { 
                return NotFound(); 
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppointmentRequest request)
        {
            try
            {
                await _service.UpdateAsync(id, User.GetUserId(), request);
                return NoContent();
            }
            catch (KeyNotFoundException) 
            { 
                return NotFound(); 
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id, User.GetUserId());
                return NoContent();
            }
            catch (KeyNotFoundException) 
            { 
                return NotFound(); 
            }
        }

        [AllowAnonymous]
        [HttpGet("confirm/{token}")]
        public async Task<IActionResult> ConfirmByToken(string token)
        {
            try
            {
                await _service.ConfirmByTokenAsync(token);

                // por enquanto, um texto HTML simples direto 
                // depois, isso será um Redirect para o front

                string htmlContent = $"<html>" +
                                        $"<body>" +
                                            $"<h1>Agendamento Confirmado!</h1>" +
                                            $"<p>Te esperamos no horário marcado.</p>" +
                                        $"</body>" +
                                     $"</html>";

                return Content(htmlContent, "text/html");
            }
            catch (KeyNotFoundException) 
            { 
                return NotFound("Link inválido."); 
            }
            catch (InvalidOperationException ex) 
            { 
                return BadRequest(ex.Message); 
            }
        }

        [AllowAnonymous]
        [HttpGet("cancel/{token}")]
        public async Task<IActionResult> CancelByToken(string token)
        {
            try
            {
                await _service.CancelByTokenAsync(token);

                string htmlContent = $"<html>" +
                                        $"<body>" +
                                            $"<h1>Agendamento Cancelado</h1>" +
                                            $"<p>Obrigado por nos avisar.</p>" +
                                        $"</body>" +
                                     $"</html>";

                return Content(htmlContent, "text/html");
            }
            catch (KeyNotFoundException) 
            { 
                return NotFound("Link inválido."); 
            }
        }
    }
}
