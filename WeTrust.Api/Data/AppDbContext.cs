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
            // ACCOUNTHEAD CONFIG
            modelBuilder.Entity<AccountHead>(entity =>
            {
                entity.ToTable("accountheads", "public");

                entity.HasKey(a => a.AccountHeadId);

                entity.Property(a => a.AccountHeadId)
                      .HasColumnName("accountheadid")
                      .ValueGeneratedOnAdd();  // 👈 important: DB generates the ID

                entity.Property(a => a.Name)
                      .HasColumnName("name")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(a => a.Category)
                      .HasColumnName("category");

                entity.Property(a => a.IsActive)
                      .HasColumnName("isactive");

                entity.Property(a => a.OpeningBalance)
                      .HasColumnName("openingbalance");

                entity.Property(a => a.OpeningBalanceType)
                      .HasColumnName("openingbalancetype");
            });



            // OTHER ENTITIES (unchanged)
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<OrganizationSetting>().HasKey(o => o.OrgId);
            modelBuilder.Entity<DocumentSequence>().HasKey(ds => ds.Id);
            modelBuilder.Entity<Document>().HasKey(d => d.DocumentId);
            modelBuilder.Entity<JournalLine>().HasKey(j => j.LineId);

            modelBuilder.Entity<Document>()
                .HasMany(d => d.JournalLines)
                .WithOne()
                .HasForeignKey(j => j.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().Property(u => u.Username).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();

            modelBuilder.Entity<OrganizationSetting>()
                .Property(o => o.OrgName).HasMaxLength(150).IsRequired();

            base.OnModelCreating(modelBuilder);
        }

    }
}
