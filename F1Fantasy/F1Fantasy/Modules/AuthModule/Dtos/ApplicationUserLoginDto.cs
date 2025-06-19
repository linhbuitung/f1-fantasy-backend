using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AuthModule.Dtos
{
    public class ApplicationUserLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}