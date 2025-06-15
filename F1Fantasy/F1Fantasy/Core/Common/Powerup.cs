using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("powerup")]
    public class Powerup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Type { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; }

        [Required, MaxLength(300)]
        public string ImgUrl { get; set; }

        // Navigation property
        public ICollection<PowerupFantasyLineup> PowerupFantasyLineups { get; set; }
    }
}