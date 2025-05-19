using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("constructor")]
    public class Constructor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string Name { get; set; }

        [Required, MaxLength(200)]
        public string Nationality { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(300)]
        public string? ImgUrl { get; set; }

        public Constructor(string name, string nationality, string code, string? imgUrl)
        {
            Name = name;
            Nationality = nationality;
            Code = code;
            ImgUrl = imgUrl;
        }

        // Navigation properties

        public ICollection<DriverPrediction> DriverPredictions { get; set; }
        public ICollection<User> Users { get; set; }
    }
}