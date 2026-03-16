using CivicPulse.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CivicPulse.Api.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Bookmark>()
            .HasIndex(b => new { b.UserId, b.EventId })
            .IsUnique();

        builder.Entity<Event>()
            .HasOne(e => e.Organizer)
            .WithMany(u => u.OrganizedEvents)
            .HasForeignKey(e => e.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Event>()
            .Property(e => e.Category)
            .HasConversion<string>();
    }
}
