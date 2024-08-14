namespace Rental_Car_Demo.Services
{
    public interface ITokenGenerator
    {
        string GenerateToken(int length);
        DateTime GetExpirationTime();
    }
}
