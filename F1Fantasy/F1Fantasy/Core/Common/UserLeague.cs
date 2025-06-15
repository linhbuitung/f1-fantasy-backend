using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("user_league")]
    public class UserLeague
    {
        [Key, Column(Order = 1)]
        public int LeagueId { get; set; }

        [Key, Column(Order = 2)]
        public int UserId { get; set; }

        [ForeignKey("LeagueId")]
        public League League { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}