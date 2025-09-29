using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

internal sealed class ShortUrlConfiguration : IEntityTypeConfiguration<ShortUrl>
{
    public void Configure(EntityTypeBuilder<ShortUrl> builder)
    {
        builder.ToTable("ShortUrls");
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.CreatedBy)
            .WithMany(b => b.ShortUrls)
            .HasForeignKey(b => b.CreatedById);
    }
}