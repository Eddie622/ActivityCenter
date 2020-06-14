using Microsoft.EntityFrameworkCore;

namespace ActivityCenter.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<PlannedActivity> PlannedActivitys { get; set; }
        public DbSet<Plan> Plans { get; set; }

    }
}