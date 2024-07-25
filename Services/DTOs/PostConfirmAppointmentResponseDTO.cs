using Newtonsoft.Json;
using Services.Models;

namespace Services.DTOs
{
    public class PostConfirmAppointmentResponseDTO
    {
        [JsonProperty("appointmentId")]
        public int AppointmentId { get; set; }
        [JsonProperty("timeSlot")]
        public DateTime TimeSlot { get; set; }
        [JsonProperty("providerName")]
        public string ProviderName { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        public static PostConfirmAppointmentResponseDTO Map(Appointment confirmedAppointment)
        {
            var responseDto = new PostConfirmAppointmentResponseDTO
            {
                AppointmentId = confirmedAppointment.Id,
                TimeSlot = confirmedAppointment.TimeSlot,
                ProviderName = confirmedAppointment.Provider.Name
            };
            responseDto.Message = $"Your appointment has been confirmed. You'll be seeing {responseDto.ProviderName} at {responseDto.TimeSlot:o}";

            return responseDto;
        }
    }
}
