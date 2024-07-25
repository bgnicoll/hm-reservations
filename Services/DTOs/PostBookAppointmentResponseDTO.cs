using Newtonsoft.Json;
using Services.Models;
using System.Text.Json.Serialization;

namespace Services.DTOs
{
    public class PostBookAppointmentResponseDTO
    {
        [JsonProperty("appointmentId")]
        public int AppointmentId { get; set; }
        [JsonProperty("timeSlot")]
        public DateTime TimeSlot { get; set; }
        [JsonProperty("providerName")]
        public string ProviderName { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }

        public static PostBookAppointmentResponseDTO Map(Appointment bookedAppointment)
        {
            var responseDto = new PostBookAppointmentResponseDTO();
            responseDto.AppointmentId = bookedAppointment.Id;
            responseDto.TimeSlot = bookedAppointment.TimeSlot;
            responseDto.ProviderName = bookedAppointment.Provider.Name;
            responseDto.Message = $"Your appointment has been reserved, but not yet confirmed. Be sure to confirm your appointment before {DateTime.UtcNow.AddMinutes(30):o} to secure your timeslot.";

            return responseDto;
        }
    }
}
