using Services.DTOs;
using Services.Models;

namespace Services
{
    public interface IAppointmentService
    {
        Task<GetAvailableAppointmentsResponseDTO> GetAvailableAppointmentsAsync(GetAvailableAppointmentsRequestDTO requestDto);
        Task<PostSubmitAvailabilityResponseDTO> GenerateAppointmentsFromProviderAvailability(User provider, PostSubmitAvailabilityRequestDTO requestDTO);
        Task<bool> AppointmentIsAvailableForReservationByClient(User client, int appointmentId);
        Task<PostBookAppointmentResponseDTO> ReserveAppointment(User client, int appointmentId);
        Task<bool> AppointmentIsAvailableToConfirmByClient(User client, int appointmentId);
        Task<PostConfirmAppointmentResponseDTO> ConfirmAppointment(User client, int appointmentId);
    }
}
