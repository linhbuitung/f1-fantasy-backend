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

        [Required]
        public int MaxPlayersNum { get; set; }

        [Required, MaxLength(100)]
        public string Type { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserProfile User { get; set; }

        // Navigation properties
        public ICollection<UserLeague> UserLeagues { get; set; }
    }
}