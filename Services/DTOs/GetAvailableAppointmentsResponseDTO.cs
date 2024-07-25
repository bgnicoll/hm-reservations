using Newtonsoft.Json;
using Services.Models;
using System.Linq;

namespace Services.DTOs
{
    public class GetAvailableAppointmentsResponseDTO : IResponseDTO
    {
        [JsonProperty("availableAppointments")]
        public List<AvailableAppointmentDTO> AvailableAppointments { get; set; }
        public static GetAvailableAppointmentsResponseDTO Map(IEnumerable<Appointment> availableAppointments)
        {
            var responseDto = new GetAvailableAppointmentsResponseDTO();
            if (availableAppointments != null && availableAppointments.Any())
            {
                responseDto.AvailableAppointments = new List<AvailableAppointmentDTO>();
                foreach (var appointment in availableAppointments)
                {
                    responseDto.AvailableAppointments.Add(
                        new AvailableAppointmentDTO
                        {
                            AppointmentId = appointment.Id,
                            TimeSlot = appointment.TimeSlot,
                            ProviderName = appointment.Provider.Name
                        });
                }
            }
            return responseDto;
        }
    }
}
