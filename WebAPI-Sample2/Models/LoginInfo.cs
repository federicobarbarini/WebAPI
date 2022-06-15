using System.ComponentModel.DataAnnotations;

namespace WebAPI_Sample2.Models
{
    public class LoginInfo
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
