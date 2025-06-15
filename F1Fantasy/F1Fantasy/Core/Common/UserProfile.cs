using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace F1Fantasy.Core.Common
{
    //public enum Role
    //{
    //    Player = 0,
    //    Admin = 1,
    //    SuperAdmin = 2
    //}

    public class UserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        public string DisplayName { get; set; }

        //[Required, MaxLength(200)]
        //public string Email { get; set; }

        //[Required]

        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        //[MaxLength(200)]
        public string Nationality { get; set; }

        //[Required, MaxLength(100)]
        //public string Password { get; set; }
        [Required]
        [Column(TypeName = "date")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "timestamp")]
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

        // Navigation properties
        public ICollection<FantasyLineup> FantasyLineups { get; set; }

        public ICollection<League> Leagues { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Prediction> Predictions { get; set; }
        public ICollection<UserLeague> UserLeagues { get; set; }
    }
}