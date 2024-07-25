using Newtonsoft.Json;

namespace Services.DTOs
{
    public class PostSubmitAvailabilityResponseDTO : IResponseDTO
    {
        [JsonProperty("appointmentSlotsCreated")]
        public int AppointmentSlotsCreated { get; set; }
    }
}
