using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class WeddingDBContext : DbContext
    {
        public WeddingDBContext(DbContextOptions<WeddingDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FamilyMember>()
                .HasOne(s => s.Family)
                .WithMany(g => g.FamilyMembers)
                .HasForeignKey(s => s.FamilyId);

            builder.Entity<Email>()
                .HasOne(s => s.Family)
                .WithMany(g => g.Emails)
                .HasForeignKey(s => s.FamilyId);

            builder.Entity<WhatsApp>()
                .HasOne(s => s.Family)
                .WithMany(g => g.WhatsApps)
                .HasForeignKey(s => s.FamilyId);
        }

        public DbSet<Family> Family { get; set; }
        public DbSet<FamilyMember> FamilyMember { get; set; }
        public DbSet<Email> Email { get; set; }
        public DbSet<WhatsApp> WhatsApp { get; set; }
    }
}
