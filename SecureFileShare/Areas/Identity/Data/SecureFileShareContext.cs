using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecureFileShare.Models;
using System.Reflection.Emit;

namespace SecureFileShare.Data;

public class SecureFileShareContext : IdentityDbContext<ApplicationUser>
{
    public SecureFileShareContext(DbContextOptions<SecureFileShareContext> options)
        : base(options)
    {
    }

    public DbSet<Models.File> Files { get; set; }
    public DbSet<Models.FileShare> FileShares { get; set; }
    public DbSet<Message> Messages { get; set; }
   

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Message>()
            .HasOne(m => m.Recipient)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.RecipientId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Models.FileShare>()
        .HasOne(fs => fs.SharedFile)
        .WithMany()
        .HasForeignKey(fs => fs.SharedFileId)
        .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Models.FileShare>()
            .HasOne(fs => fs.SharedWith)
            .WithMany()
            .HasForeignKey(fs => fs.SharedWithId)
            .OnDelete(DeleteBehavior.NoAction);


    }
}
