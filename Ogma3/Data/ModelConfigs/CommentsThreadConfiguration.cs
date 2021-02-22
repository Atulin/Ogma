using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Models;

namespace Ogma3.Data.ModelConfigs
{
    public class CommentsThreadConfiguration : BaseConfiguration<CommentsThread>
    {
        public override void Configure(EntityTypeBuilder<CommentsThread> builder)
        {
            base.Configure(builder);
            
            // CONSTRAINTS
            builder
                .Property(ct => ct.CommentsCount)
                .IsRequired()
                .HasDefaultValue(0);

            // NAVIGATION
            builder
                .HasMany(ct => ct.Comments)
                .WithOne(c => c.CommentsThread)
                .HasForeignKey(ct => ct.CommentsThreadId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(ct => ct.Subscribers)
                .WithMany(u => u.SubscribedThreads)
                .UsingEntity<CommentsThreadSubscriber>(
                    cts => cts
                        .HasOne(c => c.OgmaUser)
                        .WithMany()
                        .HasForeignKey(c => c.OgmaUserId),
                    cts => cts
                        .HasOne(c => c.CommentsThread)
                        .WithMany()
                        .HasForeignKey(c => c.CommentsThreadId)
                );
        }
    }
}