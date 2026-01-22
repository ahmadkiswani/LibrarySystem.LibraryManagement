using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Borrow> Borrows { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookCopy>()
                .HasOne(bc => bc.Book)
                .WithMany(b => b.Copies)
                .HasForeignKey(bc => bc.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Borrow>()
                .HasOne(b => b.BookCopy)
                .WithMany(c => c.BorrowRecords)
                .HasForeignKey(b => b.BookCopyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Borrow>()
                .HasOne(b => b.User)
                .WithMany(u => u.Borrows)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Borrow>()
                .Property(b => b.Status)
                .HasConversion<int>();


            modelBuilder.Entity<User>()
                .HasOne(u => u.CreatedByUser)
                .WithMany()
                .HasForeignKey(u => u.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(u => u.LastModifiedByUser)
                .WithMany()
                .HasForeignKey(u => u.LastModifiedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(u => u.DeletedByUser)
                .WithMany()
                .HasForeignKey(u => u.DeletedBy)
                .OnDelete(DeleteBehavior.NoAction);
        
               


        }

        
    }
}
