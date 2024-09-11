using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using POEPart3.Models;

namespace POEPart3.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModuleStudyRecord> ModuleStudyRecords { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated(); // This ensures that new tables are created.
        }
    }
}