using LibrarySystem.Domain.Abstractions;
using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Data
{
    public class LibraryDbContext : DbContext
    {
        private readonly IAuditUserProvider? _auditUserProvider;

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public LibraryDbContext(
            DbContextOptions<LibraryDbContext> options,
            IAuditUserProvider? auditUserProvider)
            : base(options)
        {
            _auditUserProvider = auditUserProvider;
        }

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

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<Publisher>(entity =>
            {
                entity.HasQueryFilter(x => !x.IsDeleted);
            });

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
            modelBuilder.Entity<BookCopy>(entity =>
            {
                entity.HasIndex(x => x.CopyCode).IsUnique();

                entity.HasQueryFilter(x => !x.IsDeleted);
            });
        }

        public override int SaveChanges()
        {
            ApplyAuditFields();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditFields()
        {
            var now = DateTime.UtcNow;
            var userId = _auditUserProvider?.GetCurrentUserId();

            foreach (var entry in ChangeTracker.Entries<AuditLog>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = now;
                        entry.Entity.CreatedBy = userId;
                        entry.Entity.IsDeleted = false;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = now;
                        entry.Entity.LastModifiedBy = userId;
                        break;
                    case EntityState.Deleted:
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedDate = now;
                        entry.Entity.DeletedBy = userId;
                        entry.State = EntityState.Modified;
                        break;
                }
            }
        }
    }
}
