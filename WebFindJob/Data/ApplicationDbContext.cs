using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebFindJob.Models;

namespace WebFindJob.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Recruiter>()
                .HasOne(r => r.User)
                .WithOne()
                .HasForeignKey<Recruiter>(r => r.Id);

            modelBuilder.Entity<Seeker>()
                .HasOne(s => s.User)
                .WithOne()
                .HasForeignKey<Seeker>(s => s.Id);
        }
        public DbSet<WebFindJob.Models.Seeker> Seeker { get; set; } = default!;
        public DbSet<WebFindJob.Models.Recruiter> Recruiter { get; set; } = default!;
        public DbSet<WebFindJob.Models.JobListing> JobListing { get; set; } = default!;
        public DbSet<WebFindJob.Models.JobApplication> JobApplication { get; set; } = default!;
    }
}
