using Microsoft.EntityFrameworkCore;
using mywebapp.Models;

namespace mywebapp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Note> Notes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Note>()
            .HasKey(n => n.Id);
            
        modelBuilder.Entity<Note>()
            .Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(255);
    }
}