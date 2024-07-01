using Rental_Car_Demo.Models;

namespace Rental_Car_Demo.Context
{
    public class CustomerContext
    {
        public RentCarDbContext context = new RentCarDbContext();

        public int getCustomerIdByEmail(string email)
        {
            var customer = context.Users.FirstOrDefault(x => x.Email == email);

            int id = customer.UserId;

            return id;
        }

    }
}
