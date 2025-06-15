using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Core.Common
{
    [Table("driver_prediction")]
    public class DriverPrediction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public int? GridPosition { get; set; }

        public int? FinalPosition { get; set; }

        [Required]
        public bool Crashed { get; set; }

        // Foreign keys and navigation properties
        [Required]
        public Guid PredictionId { get; set; }

        [ForeignKey(nameof(PredictionId))]
        public Prediction Prediction { get; set; }

        [Required]
        public Guid DriverId { get; set; }

        [ForeignKey(nameof(DriverId))]
        public Driver Driver { get; set; }

        [Required]
        public Guid ConstructorId { get; set; }

        [ForeignKey(nameof(ConstructorId))]
        public Constructor Constructor { get; set; }
    }
}