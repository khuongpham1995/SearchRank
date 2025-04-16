using Microsoft.EntityFrameworkCore;
using SearchRank.Domain.Entities;

namespace SearchRank.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(p => p.RowVersion).IsRowVersion().IsConcurrencyToken();
        });
        
        modelBuilder.Entity<SearchQuery>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.User)
                .WithMany(x => x.SearchQueries)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(p => p.RowVersion).IsRowVersion().IsConcurrencyToken();
        });
        
        base.OnModelCreating(modelBuilder);
    }
}