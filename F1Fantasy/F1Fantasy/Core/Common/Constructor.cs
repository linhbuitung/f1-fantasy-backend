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

        [Required, MaxLength(50)]
        public string Code { get; set; }

        [Required, Range(1,100)]
        public int Price { get; set; }
        
        [MaxLength(300)]
        public string? ImgUrl { get; set; }
        
        // Navigation properties
        [Required]
        public string CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public virtual Country Country { get; set; }

        public int? PickableItemId { get; set; }
        
        [ForeignKey(nameof(PickableItemId))]
        public virtual PickableItem? PickableItem { get; set; }
        
        public virtual ICollection<FantasyLineup> FantasyLineups { get; set; }
        
        public virtual ICollection<FantasyLineupConstructor> FantasyLineupConstructors { get; set; }
        public virtual ICollection<DriverPrediction> DriverPredictions { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        
        public virtual ICollection<RaceEntry> RaceEntries { get; set; }
    }
}