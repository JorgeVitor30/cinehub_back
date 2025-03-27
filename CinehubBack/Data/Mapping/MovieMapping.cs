using CinehubBack.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinehubBack.Data.Mapping;

public class MovieMapping: BaseMapping<Movie>
{
    public override void Setup(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("movie");
        
        builder.Property(m => m.Title).HasColumnName("title").IsRequired();

        builder.Property(m => m.Overview).HasColumnName("overview").IsRequired();

        builder.Property(m => m.VoteCount).HasColumnName("vote_count").IsRequired();

        builder.Property(m => m.VoteAverage).HasColumnName("vote_average").IsRequired();

        builder.Property(m => m.ReleaseDate).HasColumnName("release_date").IsRequired();

        builder.Property(m => m.Revenue).HasColumnName("revenue").IsRequired();

        builder.Property(m => m.RunTime).HasColumnName("runtime").IsRequired();

        builder.Property(m => m.Adult).HasColumnName("adult").IsRequired();

        builder.Property(m => m.Budget).HasColumnName("budget").IsRequired();

        builder.Property(m => m.OriginalLanguage).HasColumnName("original_language").IsRequired();

        builder.Property(m => m.Popularity).HasColumnName("popularity").IsRequired();

        builder.Property(m => m.PosterPhotoUrl).HasColumnName("poster_path").IsRequired();

        builder.Property(m => m.BackPhotoUrl).HasColumnName("backdrop_path").IsRequired();

        builder.Property(m => m.Tagline).HasColumnName("tagline").IsRequired();

        builder.Property(m => m.KeyWords).HasColumnName("keywords").IsRequired();
    }
}