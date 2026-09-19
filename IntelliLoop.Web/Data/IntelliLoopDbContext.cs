using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Entities.Models;
using IntelliLoop.Web.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IntelliLoop.Web.Data
{
    public class IntelliLoopDbContext(DbContextOptions<IntelliLoopDbContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<NoteSection> Notes { get; set; }
        public DbSet<ProcessingJob> ProcessingJobs { get; set; }
    }
}
