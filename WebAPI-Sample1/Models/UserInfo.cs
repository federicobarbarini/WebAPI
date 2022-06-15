using System.ComponentModel.DataAnnotations;

namespace WebAPI_Sample1.Models
{
    public class UserInfo
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        [Required]
        public string? DisplayName { get; set; }
        [Required]
        public string? UserName { get; set; }
        public string? Email { get; set; }
        [Required] 
        public string? Password { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
