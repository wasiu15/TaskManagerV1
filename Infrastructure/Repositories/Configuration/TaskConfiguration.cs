using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Configuration
{
    public class TaskConfiguration : IEntityTypeConfiguration<UserTask>
    {
        public void Configure(EntityTypeBuilder<UserTask> builder)
        {
            builder.HasData
            (
                new UserTask
                {
                    TaskId = Guid.NewGuid(),
                    Title = "Fitness Goals",
                    Description = "Gym workout",
                    DueDate = DateTime.Now,
                    Priority = 0,
                    Status = Domain.Status.Pending,
                },
                new UserTask
                {
                    TaskId = Guid.NewGuid(),
                    Title = "Work Project Task",
                    Description = "Research industry trends",
                    DueDate = DateTime.Now,
                    Priority = Domain.Priority.Medium,
                    Status = Domain.Status.In_progress,
                }
            );
        }
    }
}
