using System.Net.Mail;
using System.Net;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Rental_Car_Demo.Services
{
    public class 
        EmailService : IEmailService
    {

        private readonly SmtpClient _smtpClient;

        public EmailService(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public void SendEmail(string email, string subject, string message)
        {
            var mailMessage = new MailMessage("kietnvt2705@gmail.com", email, subject, message);
            _smtpClient.Send(mailMessage);
        }


        private readonly string fromAddress = "kietnvt2705@gmail.com";
        private readonly string fromPassword = "ueku bgbu qacj murs";
        private readonly string smtpHost = "smtp.gmail.com";
        private readonly int smtpPort = 587; // Adjust the port if necessary
        public void SendReturnEmail(string ownerEmail, string carName, DateTime returnDate, decimal amount)
        {
            var fromMailAddress = new MailAddress(fromAddress, "Rental Car Application");
            var toMailAddress = new MailAddress(ownerEmail);
            const string subject = "Your car has been returned";
            string body;

            if (amount == 0)
            {
                body = $"Please be informed that your car {carName} has been returned at {returnDate:dd/MM/yyyy HH:mm}. The deposit amount equal to the total amount that customer has to paid so there's no offset payment. Please go to your car's details page and confirm. Thank you!";

            }
            else if (amount > 0)
            {
                body = $"Please be informed that your car {carName} has been returned at {returnDate:dd/MM/yyyy HH:mm}. The exceed amount (the deposit amount is more than the total that customer need to paid) has been deducted from your wallet to the customer's. Please go to your car's details page and click Confirm the payment. Thank you!";
            }
            else
            {
                body = $"Please be informed that your car {carName} has been returned at {returnDate:dd/MM/yyyy HH:mm}. Please go to your wallet to check if the remaining payment has been paid and go to your car’s details page to confirm the payment. Thank you!";
            }
            using (var smtp = new SmtpClient
            {
                Host = smtpHost,
                Port = smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMailAddress.Address, fromPassword)
            })
            {
                using (var message = new MailMessage(fromMailAddress, toMailAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
        }
    }
}
