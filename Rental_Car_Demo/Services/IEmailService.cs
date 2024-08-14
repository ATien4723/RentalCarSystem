namespace Rental_Car_Demo.Services
{
    public interface IEmailService
    {
        void SendEmail(string email, string subject, string message);
        void SendReturnEmail(string ownerEmail, string carName, DateTime returnDate, decimal amount);
    }
}
