using Domain.Models;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace TaskManager.Infrastructure
{
    public static class Util
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

        //  THIS METHOD HELPS TO CHECK IF LIST OF USERTASK OBJECTS CONTAINS A USERTASK OBJECT 
        public static bool IsListContainTask(List<UserTask> userTasks, UserTask task)
        {
            foreach (var item in userTasks)
            {
                if (item.TaskId == task.TaskId)
                    return true;
            }
            return false;
        }

        public static bool EmailIsValid(string email)
        {
            return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        public static string StringHasher(string input)
        {
            return ComputeSha256Hash(input);
        }

        private static string ComputeSha256Hash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


        public static void SendEmail(string email, string type, string input_message)
        {
            try
            {
                
                //var subject = type.Replace('_',' '); var template = input_message;
                //MailMessage message = new MailMessage();
                //SmtpClient smtp = new SmtpClient();
                //message.From = new MailAddress("mail@autoarbs.com");
                //message.To.Add(new MailAddress(email));
                //message.Subject = subject;
                //message.IsBodyHtml = true; //to make message body as html  
                //message.Body = template;
                ////message.Body = "Your OTP code is "+otp;
                //smtp.Port = 587;
                //smtp.Host = "smtp.gmail.com"; //for gmail host  
                //smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new NetworkCredential("autoarbs.mail@gmail.com", "jvvwqviukumwtwtm");
                //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //smtp.Send(message);
            }
            catch (Exception)
            {
                
            }
        }
    }
}