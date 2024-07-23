namespace Rental_Car_Demo.Repository.BookingRepository
{
    public static class BookingHelper
    {
        public static int GetDaysBetween(DateTime startDate, DateTime endDate)
        {
            TimeSpan difference = endDate - startDate;
            return (int)Math.Ceiling(difference.TotalDays);
        }
        public static decimal GetTotalPrice(decimal baseprice, DateTime startDate, DateTime endDate)
        {
            TimeSpan difference = endDate - startDate;
            return baseprice * (int)Math.Ceiling(difference.TotalDays);
        }
        public static decimal GetTotalDeposit(decimal deposit, DateTime startDate, DateTime endDate)
        {
            TimeSpan difference = endDate - startDate;
            return deposit * (int)Math.Ceiling(difference.TotalDays);
        }
    }
}
