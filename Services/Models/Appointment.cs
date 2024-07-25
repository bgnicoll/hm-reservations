using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public DateTime TimeSlot { get; set; }
        public User Provider { get; set; }
        public User Client { get; set; }
        public DateTime? ReservedDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }

        public int ProviderUserId { get; set; }
        public int? ClientUserId { get; set; }
    }
}
