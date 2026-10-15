using IntelliLoop.Infrastructure.Entities;
using IntelliLoop.Infrastructure.Entities.Models;
using IntelliLoop.Web.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IntelliLoop.Infrastructure
{
    public class IntelliLoopDbContext(DbContextOptions<IntelliLoopDbContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<NoteSection> Notes { get; set; }
        public DbSet<ProcessingJob> ProcessingJobs { get; set; }

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
