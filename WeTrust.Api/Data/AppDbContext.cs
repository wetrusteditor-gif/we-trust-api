using Microsoft.EntityFrameworkCore;
using WeTrust.Api.Models;

namespace WeTrust.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<OrganizationSetting> OrganizationSettings { get; set; }
        public DbSet<AccountHead> AccountHeads { get; set; }
        public DbSet<DocumentSequence> DocumentSequences { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<JournalLine> JournalLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define primary keys explicitly
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<OrganizationSetting>().HasKey(o => o.OrgId);
            modelBuilder.Entity<AccountHead>().HasKey(a => a.AccountHeadId);
            modelBuilder.Entity<DocumentSequence>().HasKey(ds => ds.Id);
            modelBuilder.Entity<Document>().HasKey(d => d.DocumentId);
            modelBuilder.Entity<JournalLine>().HasKey(j => j.LineId);

            // Configure relationships (optional but helpful)
            modelBuilder.Entity<Document>()
                .HasMany(d => d.JournalLines)
                .WithOne()
                .HasForeignKey(j => j.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure column lengths / types if desired
            modelBuilder.Entity<User>().Property(u => u.Username).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();

            modelBuilder.Entity<AccountHead>().Property(a => a.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<OrganizationSetting>().Property(o => o.OrgName).HasMaxLength(150).IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
