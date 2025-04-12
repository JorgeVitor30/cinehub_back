using CinehubBack.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinehubBack.Data.Mapping;

public class FavoritesMapping: BaseMapping<Favorites>
{
    public override void Setup(EntityTypeBuilder<Favorites> builder)
    {
        builder.ToTable("favorites");

        builder.HasKey(f => new { f.UserId, f.MovieId });

        builder.Property(f => f.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(f => f.MovieId)
            .HasColumnName("movie_id")
            .IsRequired();

        builder.HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.Movie)
            .WithMany(m => m.Favorites) 
            .HasForeignKey(f => f.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}