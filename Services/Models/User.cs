using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<ApiKey> ApiKeys { get; set; }
        public ICollection<Appointment> ProviderAppointments { get; set; }
        public ICollection<Appointment> ClientAppointments { get; set; }
    }
}
