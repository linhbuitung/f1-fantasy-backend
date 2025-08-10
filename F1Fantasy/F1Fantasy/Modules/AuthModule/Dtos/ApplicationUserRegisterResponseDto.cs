using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AuthModule.Dtos
{
    public class ApplicationUserRegisterResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; }

        public string Role { get; set; }
    }
}