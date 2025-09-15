using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("league")]
    public class League
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, Range(1, 100)]
        public int MaxPlayersNum { get; set; }

        [Required]
        public LeagueType Type { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public int OwnerId { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public virtual ApplicationUser User { get; set; }

        // Navigation properties
        public virtual ICollection<UserLeague> UserLeagues { get; set; }
    }
    
    // Enum for type of league
    public enum LeagueType
    {
        Public,
        Private
    }
}