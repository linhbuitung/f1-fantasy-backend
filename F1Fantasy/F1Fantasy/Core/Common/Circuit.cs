using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace F1Fantasy.Core.Common
{
    [Table("circuit")]
    public class Circuit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string CircuitName { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; }

        [Required]
        [Column(TypeName = "decimal(9,7)")]
        public decimal Latitude { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,7)")]
        public decimal Longtitude { get; set; }

        [Required, MaxLength(200)]
        public string Locality { get; set; }

        [Required]
        public string CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public Country Country { get; set; }

        [MaxLength(300)]
        public string? ImgUrl { get; set; }

        // Navigation properties
        public virtual ICollection<Prediction> Predictions { get; set; }

        public virtual ICollection<Race> Races { get; set; }
    }
}