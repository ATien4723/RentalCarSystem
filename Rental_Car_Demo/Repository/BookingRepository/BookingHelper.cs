namespace Rental_Car_Demo.Repository.BookingRepository
{
    public static class BookingHelper
    {
        public static int GetDaysBetween(DateTime startDate, DateTime endDate)
        {
            TimeSpan difference = endDate - startDate;

            if (difference.TotalDays > 0)
            {
                int fullDays = (int)difference.TotalDays;
                double remainingHours = difference.TotalHours - (fullDays * 24);

                if (remainingHours < 12)
                {
                    return fullDays +0; 
                }
                return fullDays + 1; 
            }
            return -1;
        }

        public static decimal GetTotalPrice(decimal baseprice, DateTime startDate, DateTime endDate)
        {
            TimeSpan difference = endDate - startDate;

            if (difference.TotalDays > 0 && baseprice > 0)
            {
                int fullDays = (int)difference.TotalDays;
                double remainingHours = difference.TotalHours - (fullDays * 24);

                decimal rentalDays = fullDays;
                if (remainingHours < 12)
                {
                    rentalDays += 0.5m; 
                }
                else
                {
                    rentalDays += 1m; 
                }

                return baseprice * rentalDays;
            }
            return -1;
        }

        public static decimal GetTotalDeposit(decimal deposit, DateTime startDate, DateTime endDate)
        {
            TimeSpan difference = endDate - startDate;

            if (difference.TotalDays > 0 && deposit > 0)
            {
                int fullDays = (int)difference.TotalDays;
                double remainingHours = difference.TotalHours - (fullDays * 24);

                decimal rentalDays = fullDays;
                if (remainingHours < 12)
                {
                    rentalDays += 0.5m; 
                }
                else
                {
                    rentalDays += 1m; 
                }

                return deposit * rentalDays;
            }
            return -1;
        }

        public static decimal GetTotalPriceFromToday(decimal baseprice, DateTime startDate, DateTime endDate)
        {
            DateTime today = DateTime.Today;
            DateTime actualEndDate = today < endDate ? today : endDate;
            DateTime startDateOnly = startDate.Date;
            DateTime actualEndDateOnly = actualEndDate.Date;
            TimeSpan difference = actualEndDateOnly - startDateOnly;

            if (difference.TotalDays > 0)
            {
                int fullDays = (int)difference.TotalDays;
                double remainingHours = difference.TotalHours - (fullDays * 24);

                decimal rentalDays = fullDays;
                if (remainingHours < 12)
                {
                    rentalDays += 0.5m; 
                }
                else
                {
                    rentalDays += 1m; 
                }

                return baseprice * rentalDays;
            }
            return -1;
        }
    }
}
