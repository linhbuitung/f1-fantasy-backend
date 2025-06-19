using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("constructor")]
    public class Constructor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, MaxLength(300)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(300)]
        public string? ImgUrl { get; set; }

        // constructor
        public Constructor(string name, string code, string? imgUrl)
        {
            Name = name;
            Code = code;
            ImgUrl = imgUrl;
        }

        // Navigation properties
        [Required]
        public string NationalityId { get; set; }

        [ForeignKey(nameof(NationalityId))]
        public Nationality Nationality { get; set; }

        public ICollection<DriverPrediction> DriverPredictions { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}