using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations
{
    public class DriverPredictionConfiguration : IEntityTypeConfiguration<DriverPrediction>
    {
        public void Configure(EntityTypeBuilder<DriverPrediction> entity)
        {
            entity.HasOne(dp => dp.Prediction)
                .WithMany(p => p.DriverPredictions)
                .HasForeignKey(dp => dp.PredictionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}