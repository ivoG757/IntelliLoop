using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IntelliLoop.Web.Data
{
    public class IntelliLoopDbContext(DbContextOptions<IntelliLoopDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<NoteSection> Notes { get; set; }
        public DbSet<ProcessingJob> ProcessingJobs { get; set; }
    }
}
