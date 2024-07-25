using API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTOs;
using Services.Models;

namespace API.Controllers
{
    [Route("api/appointment")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [ApiKeyAuthorize([RoleEnum.Client, RoleEnum.Provider])]
        [HttpGet("open")]
        public async Task<IActionResult> GetAvailableAppointments([FromQuery] GetAvailableAppointmentsRequestDTO requestDto)
        {
            var validationErrors = requestDto.Validate();
            if (validationErrors.Any())
            {
                return BadRequest(validationErrors);
            }

            var availableAppointments = await _appointmentService.GetAvailableAppointmentsAsync(requestDto);

            return Ok(availableAppointments);
        }

        [ApiKeyAuthorize([RoleEnum.Client])]
        [HttpPost("reserve/{appointmentId}")]
        public async Task<IActionResult> ReserveAppointment(int appointmentId)
        {
            var user = HttpContext.Items["User"] as User;
            var userCanBookAppointment = await _appointmentService.AppointmentIsAvailableForReservationByClient(user, appointmentId);

            if (!userCanBookAppointment)
            {
                return BadRequest("Appointment is not available for reservation. This slot may have already been booked or you may have a conflict with this time slot");
            }

            var bookedAppointment = await _appointmentService.ReserveAppointment(user, appointmentId);
            if (bookedAppointment == null)
            {
                return BadRequest("Appointment is not available for reservation. This slot may have already been booked or you may have a conflict with this time slot");
            }
            return Ok(bookedAppointment);
        }

        [ApiKeyAuthorize([RoleEnum.Client])]
        [HttpPost("confirm/{appointmentId}")]
        public async Task<IActionResult> ConfirmAppointment(int appointmentId)
        {
            var user = HttpContext.Items["User"] as User;
            var userCanConfirmAppointment = await _appointmentService.AppointmentIsAvailableToConfirmByClient(user, appointmentId);

            if (!userCanConfirmAppointment)
            {
                return BadRequest("Appointment is not available for confirmation. This slot may have already been booked by someone else " +
                    "or you may have a conflict with this time slot");
            }

            var confirmedAppointment = await _appointmentService.ConfirmAppointment(user, appointmentId);
            if (confirmedAppointment == null)
            {
                return BadRequest("Appointment is not available for confirmation. This slot may have already been booked or you may have a conflict with this time slot");
            }
            return Ok(confirmedAppointment);
        }

    }
}
