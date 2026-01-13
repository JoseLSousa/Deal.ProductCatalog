using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(t => t.TagId);

            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasOne(t => t.Product)
                     .WithMany(p => p.Tags)
                     .HasForeignKey(t => t.ProductId)
                     .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
