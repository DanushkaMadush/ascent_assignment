using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(r => r.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(r => r.ExpiresAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(r => r.RevokedAt)
                .HasColumnType("datetime2");

            builder.HasIndex(r => r.Token)
                .IsUnique();

            builder.HasIndex(r => r.UserId);

            builder.HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
