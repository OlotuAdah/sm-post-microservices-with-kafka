using Microsoft.EntityFrameworkCore;
using Post.Common.Entities;

namespace Post.Query.Infrastructure.DataAccess;
public class ReadDatabaseContext(DbContextOptions<ReadDatabaseContext> options) : DbContext(options)
{
    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Explicitly set the schema to dbo for both entities
        modelBuilder.Entity<PostEntity>().ToTable("Post", "dbo");
        modelBuilder.Entity<CommentEntity>().ToTable("Comment", "dbo");

        // Configure relationships if needed
        modelBuilder.Entity<PostEntity>()
            .HasMany(p => p.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("YourConnectionStringHere"); // Replace with actual connection string
        }
    }
}