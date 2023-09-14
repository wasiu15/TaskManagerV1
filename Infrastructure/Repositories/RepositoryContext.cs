using Domain.Models;
using Infrastructure.Repositories.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options): base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TaskConfiguration());
        }

        public DbSet<UserTask>? Tasks { get; set; }
        public DbSet<Project>? Projects { get; set; }
        public DbSet<User>? Users { get; set; }
        public DbSet<Notification>? Notifications { get; set; }
    }
}
