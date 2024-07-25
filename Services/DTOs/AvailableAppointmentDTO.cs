namespace Services.DTOs
{
    public class AvailableAppointmentDTO
    {
        public int AppointmentId { get; set; }
        public DateTime TimeSlot { get; set; }
        public string ProviderName { get; set; }
    }
}
