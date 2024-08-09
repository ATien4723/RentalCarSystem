namespace Rental_Car_Demo.Repository.BookingRepository
{
    public class BookingHelper
    {
        public static int GetDaysBetween(DateTime startDate, DateTime endDate)
        {
            TimeSpan difference = endDate - startDate;
            int result = (int)Math.Ceiling(difference.TotalDays);
            if (difference.TotalDays > 0)
            {
                return result;
            }
            return -1;
        }
        public static decimal GetTotalPrice(decimal baseprice, DateTime startDate, DateTime endDate)
        {
            TimeSpan difference = endDate - startDate;
            if (difference.TotalDays > 0 && baseprice > 0)
            {
                return baseprice * (int)Math.Ceiling(difference.TotalDays);
            }
            return -1;
            
        }
        public static decimal GetTotalDeposit(decimal deposit, DateTime startDate, DateTime endDate)
        {
            TimeSpan difference = endDate - startDate;
            if (difference.TotalDays > 0 && deposit > 0)
            {
                return deposit * (int)Math.Ceiling(difference.TotalDays);
            }
            return -1;
        }
        public static decimal GetTotalPriceFromToday(decimal baseprice, DateTime startDate, DateTime endDate)
        {
            DateTime today = DateTime.Today;
            DateTime actualEndDate = today < endDate ? today : endDate;
            DateTime startDateOnly = startDate.Date;
            DateTime actualEndDateOnly = actualEndDate.Date;
            int days = (actualEndDateOnly - startDateOnly).Days;
            days = days < 0 ? 0 : days;

            return baseprice > 0 ? baseprice * days : 0;
        }
    }
}
