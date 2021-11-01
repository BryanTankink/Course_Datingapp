using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class RegisterDto
    {
        [Required]
        [MinLength(length: 5, ErrorMessage = "Minimum username length is 5")]
        public string Username { get; set; } = "";

        [Required] public string KnownAs { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }

        [Required]
        [MinLength(length: 5, ErrorMessage = "Minimum password length is 5")]
        public string Password { get; set; } = "";
    }
}