using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("user")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string DisplayName { get; set; }

        [Required, MaxLength(200)]
        public string Email { get; set; }

        [Required]
        [Column("date_of_birth", TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        [Required, MaxLength(200)]
        public string Nationality { get; set; }

        [Required, MaxLength(100)]
        public string Password { get; set; }

        [Required]
        [Column("created_at", TypeName = "date")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("last_login", TypeName = "timestamp")]
        public DateTime LastLogin { get; set; }

        [Required]
        public bool AcceptNotification { get; set; }

        [Required]
        public int LoginStreak { get; set; }

        [Required]
        public int ConstructorId { get; set; }

        [ForeignKey("ConstructorId")]
        public Constructor Constructor { get; set; }

        [Required]
        public int DriverId { get; set; }

        [ForeignKey("DriverId")]
        public Driver Driver { get; set; }

        [Required, MaxLength(100)]
        public required string Role { get; set; }

        // Navigation properties
        public ICollection<FantasyLineup> FantasyLineups { get; set; }

        public ICollection<League> Leagues { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Prediction> Predictions { get; set; }
        public ICollection<UserLeague> UserLeagues { get; set; }
    }
}