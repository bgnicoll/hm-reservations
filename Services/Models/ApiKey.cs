using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    public class ApiKey
    {
        [Key]
        public int Id { get; set; }
        public string Key { get; set; }
        public DateTime Expiration { get; set; }
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }
    }
}
