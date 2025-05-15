using CinehubBack.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinehubBack.Data.Mapping;

public class RateMapping: BaseMapping<Model.Rate>
{
    public override void Setup(EntityTypeBuilder<Model.Rate> builder)
    {
        builder.ToTable("rate");
    
        builder.HasKey(r => new { r.UserId, r.MovieId });

        builder.Property(r => r.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(r => r.MovieId)
            .HasColumnName("movie_id")
            .IsRequired();
        
        builder.Property(r => r.RateValue).HasColumnName("rate_value").IsRequired();
        
        builder.Property(r => r.Comment).HasColumnName("comment");

        builder.HasOne(r => r.User)
            .WithMany(u => u.Rates)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Movie)
            .WithMany(m => m.Rates) 
            .HasForeignKey(r => r.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}