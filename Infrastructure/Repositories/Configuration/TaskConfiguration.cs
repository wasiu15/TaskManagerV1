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
                    DueDate = new DateOnly(2023, 11, 10),
                    Priority = Domain.Priority.Medium,
                    Status = Domain.Status.Pending,
                },
                new UserTask
                {
                    TaskId = Guid.NewGuid(),
                    Title = "Work Project Task",
                    Description = "Research industry trends",
                    DueDate = new DateOnly(2023, 10, 12),
                    Priority = Domain.Priority.Medium,
                    Status = Domain.Status.In_progress,
                }
            );
        }
    }
}
