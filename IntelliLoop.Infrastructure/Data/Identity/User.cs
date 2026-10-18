using IntelliLoop.Core.Entities.Models;
using Microsoft.AspNetCore.Identity;
namespace IntelliLoop.Infrastructure.Data.Identity
{
    public class User : IdentityUser
    {
        public ICollection<Lecture> Lecture { get; set; } = new List<Lecture>();
        public ICollection<ProcessingJob> ProcessingJobs { get; set; } = new List<ProcessingJob>();
    }
}
