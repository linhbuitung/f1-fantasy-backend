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
    public class ApplicationUser : IdentityUser<int>
    {
        [MaxLength(100)]
        public string? DisplayName { get; set; }

        [Column(TypeName = "date")]
        public DateOnly? DateOfBirth { get; set; }

        public bool AcceptNotification { get; set; }

        public int? LoginStreak { get; set; }

        public DateTime? LastLogin { get; set; }

        public int? ConstructorId { get; set; }

        [ForeignKey(nameof(ConstructorId))]
        public virtual Constructor? Constructor { get; set; }

        public int? DriverId { get; set; }

        [ForeignKey(nameof(DriverId))]
        public virtual Driver? Driver { get; set; }

        // Navigation properties
        public string? CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public virtual Country? Country { get; set; }

        public virtual ICollection<FantasyLineup> FantasyLineups { get; set; }

        public virtual ICollection<League> Leagues { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Prediction> Predictions { get; set; }
        public virtual ICollection<UserLeague> UserLeagues { get; set; }

        public  virtual ICollection<ApplicationUserClaim> Claims { get; set; }
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}