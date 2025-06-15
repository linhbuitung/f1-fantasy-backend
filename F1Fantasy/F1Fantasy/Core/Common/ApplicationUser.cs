using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata;
using F1Fantasy.Core.Auth;

namespace F1Fantasy.Core.Common
{
    //public enum Role
    //{
    //    Player = 0,
    //    Admin = 1,
    //    SuperAdmin = 2
    //}
    [Table("application_user")]
    public class ApplicationUser : IdentityUser<Guid>
    {
        [MaxLength(100)]
        public string DisplayName { get; set; }

        //[Required, MaxLength(200)]
        //public string Email { get; set; }

        //[Required]

        [Column(TypeName = "date")]
        public DateOnly DateOfBirth { get; set; }

        //[MaxLength(200)]
        public string Nationality { get; set; }

        //[Required, MaxLength(100)]
        //public string Password { get; set; }
        [Column(TypeName = "timestamp with time zone")]
        public DateTime CreatedAt { get; set; }

        public bool AcceptNotification { get; set; }

        public int LoginStreak { get; set; }

        public DateTime? LastLogin { get; set; }

        public Guid? ConstructorId { get; set; }

        [ForeignKey(nameof(ConstructorId))]
        public Constructor Constructor { get; set; }

        public Guid? DriverId { get; set; }

        [ForeignKey(nameof(DriverId))]
        public Driver Driver { get; set; }

        // Navigation properties
        public ICollection<FantasyLineup> FantasyLineups { get; set; }

        public ICollection<League> Leagues { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Prediction> Predictions { get; set; }
        public ICollection<UserLeague> UserLeagues { get; set; }

        public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}