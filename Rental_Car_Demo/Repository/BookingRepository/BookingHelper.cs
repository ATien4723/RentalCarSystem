using Rental_Car_Demo.Models;

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
                    return fullDays + 0;
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
            TimeSpan difference = actualEndDate - startDate;

            if (difference.TotalDays > 0 && baseprice >0)
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
    }
}