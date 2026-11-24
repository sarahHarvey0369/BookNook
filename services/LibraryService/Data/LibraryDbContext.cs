using BookNook.LibraryService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookNook.LibraryService.Data;

public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
    public DbSet<LibraryBook> LibraryBooks => Set<LibraryBook>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LibraryBook>(entity =>
        {
            entity.HasKey(book => book.Id);

            entity.Property(book => book.ExternalBookId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(book => book.CatalogSource)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(book => book.Title)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(book => book.Authors)
                .HasColumnType("text[]");

            entity.Property(book => book.ISBN)
                .HasMaxLength(20);

            entity.Property(book => book.Publisher)
                .HasMaxLength(255);

            entity.Property(book => book.Status)
                .HasConversion<string>()
                .HasMaxLength(30);

            // Prevent a user from adding the same catalog book twice.
            entity.HasIndex(book => new
            {
                book.UserId,
                book.ExternalBookId,
                book.CatalogSource
            }).IsUnique();
        });
    }
}