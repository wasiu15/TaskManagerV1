using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Dtos;

namespace TaskManager.Infrastructure
{
    public static class Helper
    {
        public static DateOnly GetLastDayOfCurrentWeek()
        {
            var currentDay = DateTime.UtcNow.DayOfWeek.ToString();
            var curentDate = DateTime.UtcNow;
            switch (currentDay)
            {
                case "Sunday":
                    curentDate = DateTime.UtcNow.AddDays(6);
                    break;
                case "Monday":
                    curentDate = DateTime.UtcNow.AddDays(5);
                    break;
                case "Tuesday":
                    curentDate = DateTime.UtcNow.AddDays(4);
                    break;
                case "Wednesday":
                    curentDate = DateTime.UtcNow.AddDays(3);
                    break;
                case "Thursday":
                    curentDate = DateTime.UtcNow.AddDays(2);
                    break;
                case "Friday":
                    curentDate = DateTime.UtcNow.AddDays(1);
                    break;
                case "Saturday":
                    curentDate = DateTime.UtcNow;
                    break;
                default:
                    curentDate = DateTime.UtcNow;
                    break;
            }
            return DateOnly.FromDateTime(curentDate);
        }

    }
}
