namespace Rental_Car_Demo.Validation
{
    public interface IEmailService
    {
        void SendEmail(string email, string subject, string message);
    }
}
