using CinehubBack.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinehubBack.Data.Mapping;

public class UserMapping : BaseMapping<User>
{
    public override void Setup(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");

        builder.Property(u => u.Name).HasColumnName("name").IsRequired();

        builder.Property(u => u.Email).HasColumnName("email").IsRequired();

        builder.Property(u => u.Role).HasColumnName("role").IsRequired();
        
        builder.Property(u => u.Password).HasColumnName("password").IsRequired();
    }
}