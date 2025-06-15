using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AuthModule.Dtos
{
    public class ApplicationUserRegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [MaxLength(100)]
        public string DisplayName { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string Nationality { get; set; }
    }
}