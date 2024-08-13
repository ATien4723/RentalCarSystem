using Rental_Car_Demo.Models;

namespace Rental_Car_Demo.Repository.BookingRepository
{
    public static class BookingHelper
    {
        public static int GetDaysBetween(DateTime startDate, DateTime endDate)
        {
            TimeSpan difference = endDate - startDate;
            return (int)Math.Ceiling(difference.TotalDays);
        }
        public static string NumberOfDays (DateTime startDate, DateTime endDate)
        {
            string result = "";
            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);
            var totalHours = (int)Math.Ceiling((endDate - startDate).TotalHours);
            if (totalHours < 12)
            {
                result = "Half a day";
            }
            else
            {
                result = $"{numberOfDays}";
            }
            return result;
        }
        public static decimal TotalPrice(decimal baseprice, DateTime startDate, DateTime endDate)
        {
            decimal result = 0;
            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);
            var totalHours = (int)Math.Ceiling((endDate - startDate).TotalHours);
            if (totalHours < 12)
            {
                result = baseprice * 0.5m;
            }
            else
            {
                result = numberOfDays * baseprice;
            }
            return result;
        }

        public static decimal TotalDeposit(decimal deposit, DateTime startDate, DateTime endDate)
        {
            decimal result = 0;
            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);
            var totalHours = (int)Math.Ceiling((endDate - startDate).TotalHours);
            if (totalHours < 12)
            {
                result = deposit * 0.5m;
            }
            else
            {
                result = numberOfDays * deposit;
            }
            return result;
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
        public static decimal GetTotalPriceFromToday(decimal baseprice, DateTime startDate, DateTime endDate)
        {
            DateTime today = DateTime.Today;
            DateTime actualEndDate = today < endDate ? today : endDate;
            DateTime startDateOnly = startDate.Date;
            DateTime actualEndDateOnly = actualEndDate.Date;
            int days = (actualEndDateOnly - startDateOnly).Days;
            days = days < 0 ? 0 : days;

            return baseprice * days;
        }
    }
}
