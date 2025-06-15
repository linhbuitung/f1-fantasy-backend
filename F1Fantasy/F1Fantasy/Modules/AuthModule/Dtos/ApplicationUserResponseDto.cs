using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.AuthModule.Dtos
{
    public class ApplicationUserResponseDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }
}