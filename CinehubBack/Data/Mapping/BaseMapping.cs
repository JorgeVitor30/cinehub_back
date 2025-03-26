using CinehubBack.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace CinehubBack.Data.Mapping;

public abstract class BaseMapping<T> : IEntityTypeConfiguration<T>
    where T : BaseEntity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasValueGenerator<GuidValueGenerator>().HasColumnName("id");

        builder.Property(e => e.CreatedAt).HasColumnName("created_at").ValueGeneratedOnAdd();

        builder
            .Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .ValueGeneratedOnAddOrUpdate();

        Setup(builder);
    }

    public abstract void Setup(EntityTypeBuilder<T> builder);
}