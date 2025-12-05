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
    }
}

