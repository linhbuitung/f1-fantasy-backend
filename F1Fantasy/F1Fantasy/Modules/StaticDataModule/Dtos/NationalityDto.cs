using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.StaticDataModule.Dtos
{
    public class NationalityDto
    {
        [MaxLength(100)]
        public string NationalityId { get; set; }

        [Required]
        public List<string> Names { get; set; }
    }
}