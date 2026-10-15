using IntelliLoop.Core.Entities.Models;
using IntelliLoop.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IntelliLoop.Infrastructure.Data
{
    public class IntelliLoopDbContext(DbContextOptions<IntelliLoopDbContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<Lecture> Lectures { get; set; } = null!;
        public DbSet<Questions> Questions { get; set; } = null!;
        public DbSet<NoteSection> Notes { get; set; } = null!;
        public DbSet<ProcessingJob> ProcessingJobs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProcessingJob>()
                .HasOne(job => job.Lecture)
                .WithOne(lecture => lecture.ProcessingJob)
                .HasForeignKey<ProcessingJob>(job => job.LectureId);
        }
    }
}
