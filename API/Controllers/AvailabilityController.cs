using API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTOs;
using Services.Models;

namespace API.Controllers
{
    [Route("api/availability")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        public AvailabilityController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [ApiKeyAuthorize([RoleEnum.Provider])]
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAvailability([FromBody] PostSubmitAvailabilityRequestDTO requestDto)
        {
            var validationErrors = requestDto.Validate();
            if (validationErrors.Any())
            {
                return BadRequest(validationErrors);
            }

            var user = HttpContext.Items["User"] as User;

            var responseDto = await _appointmentService.GenerateAppointmentsFromProviderAvailability(user, requestDto);
            return Ok(responseDto);
        }

    }
}
