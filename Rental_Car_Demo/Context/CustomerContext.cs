using Rental_Car_Demo.Models;

namespace Rental_Car_Demo.Context
{
    public class CustomerContext
    {
        public RentCarDbContext context = new RentCarDbContext();
        private readonly ILogger<CustomerContext> _logger;
        public int getCustomerIdByEmail(string email)
        {
            //_logger.LogDebug("Email passed to getCustomerIdByEmail: {Email}", email);
            var customer = context.Users.FirstOrDefault(x => x.Email == email);

            if (customer == null)
            {
                return -1; // or throw an exception
            }

            return customer.UserId;
        }

    }
}
