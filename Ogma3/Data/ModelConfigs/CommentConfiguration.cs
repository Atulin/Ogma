using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Models;

namespace Ogma3.Data.ModelConfigs
{
    public class CommentConfiguration : BaseConfiguration<Comment>
    {
        public override void Configure(EntityTypeBuilder<Comment> builder)
        {
            base.Configure(builder);
            
            // CONSTRAINTS
            builder
                .Property(c => c.DateTime)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder
                .Property(c => c.LastEdit)
                .HasDefaultValue(null);

            builder
                .Property(c => c.Body)
                .IsRequired()
                .HasMaxLength(CTConfig.CComment.MaxBodyLength);

            builder
                .Property(c => c.DeletedBy)
                .HasDefaultValue(null);

            builder
                .Property(c => c.DeletedByUserId)
                .HasDefaultValue(null);

            builder
                .Property(c => c.EditCount)
                .HasDefaultValue(0);
            
            // NAVIGATION
            builder
                .HasOne(c => c.Author)
                .WithMany();
            
            builder
                .HasOne(c => c.DeletedByUser)
                .WithMany();
            
            builder
                .HasMany(c => c.Reports)
                .WithOne(r => r.Comment)
                .HasForeignKey(r => r.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}