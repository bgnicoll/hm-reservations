using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public RoleEnum Role { get; set; }
    }
}
