using IntelliLoop.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IntelliLoop.Web.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<NoteSection> Notes { get; set; }
    }
}
