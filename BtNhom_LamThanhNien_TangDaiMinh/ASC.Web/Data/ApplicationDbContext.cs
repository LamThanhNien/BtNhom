using ASC.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASC.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<MasterDataKey> MasterDataKeys { get; set; }
        public DbSet<MasterDataValue> MasterDataValues { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<MasterDataKey>()
                .HasIndex(m => m.Key)
                .IsUnique();

            builder.Entity<MasterDataValue>()
                .HasOne(m => m.MasterDataKey)
                .WithMany(k => k.MasterDataValues)
                .HasForeignKey(m => m.MasterDataKeyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}