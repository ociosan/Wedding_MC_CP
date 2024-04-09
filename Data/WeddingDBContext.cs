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

        }

        public DbSet<Family> Family { get; set; }
        public DbSet<FamilyMember>  FamilyMember { get; set; }
    }
}
