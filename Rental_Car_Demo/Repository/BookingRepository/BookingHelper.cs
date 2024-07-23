namespace Rental_Car_Demo.Repository.BookingRepository
{
    public static class BookingHelper
    {
        public static int GetDaysBetween(DateTime startDate, DateTime endDate)
        {
            return (endDate - startDate).Days;
        }
        public static decimal GetTotalPrice(decimal baseprice, DateTime startDate, DateTime endDate)
        {
            return baseprice * (endDate - startDate).Days;
        }
        public static decimal GetTotalDeposit(decimal deposit, DateTime startDate, DateTime endDate)
        {
            return deposit * (endDate - startDate).Days;
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
