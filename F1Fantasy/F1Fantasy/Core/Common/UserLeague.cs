using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("user_league")]
    public class UserLeague
    {
        [Key, Column(Order = 1)]
        public Guid LeagueId { get; set; }

        [Key, Column(Order = 2)]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(LeagueId))]
        public League League { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}