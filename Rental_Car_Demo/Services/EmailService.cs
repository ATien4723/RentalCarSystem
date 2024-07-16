using System.Net.Mail;
using System.Net;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Rental_Car_Demo.Validation
{
    public class EmailService:IEmailService
    {
        public void SendEmail(string email, string subject, string message)
        {
            
            // Cấu hình thông tin SMTP
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                
                Credentials = new NetworkCredential("kietnvt2705@gmail.com", "ueku bgbu qacj murs")
            };

             client.Send(
                new MailMessage(from: "kietnvt2705@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
            
        }
    }
}
