using Microsoft.EntityFrameworkCore;
using Rental_Car_Demo.Models;

namespace Rental_Car_Demo.Repository.BookingRepository
{
    public class BookingDAO
    {
        private static BookingDAO instance = null;
        private static readonly object instanceLock = new object();
        public static BookingDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new BookingDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<Booking> GetBookingList()
        {
            var bookings = new List<Booking>();
            try
            {
                using var context = new RentCarDbContext();
                bookings = context.Bookings.Include(b => b.BookingInfoId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return bookings;
        }

        public Booking GetBookingById(int bookingNo)
        {
            Booking booking = null;
            try
            {
                using var context = new RentCarDbContext();
                booking = context.Bookings.Include(b => b.BookingInfoId).SingleOrDefault(b => b.BookingNo == bookingNo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return booking;
        }

        public void Create(Booking booking)
        {
            try
            {
                Booking _booking = GetBookingById(booking.BookingNo);
                if (_booking == null)
                {
                    using var context = new RentCarDbContext();
                    context.Bookings.Add(booking);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("This booking is already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string Edit(Booking booking)
        {
            try
            {
                using var context = new RentCarDbContext();
                Booking _booking = GetBookingById(booking.BookingNo);
                if (_booking == null)
                {
                    return "This booking does not already exist.";
                }

                context.Bookings.Update(booking);
                context.SaveChanges();
                return null;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int bookingNo)
        {
            try
            {
                Booking booking = GetBookingById(bookingNo);
                if (booking != null)
                {
                    using var context = new RentCarDbContext();
                    context.Bookings.Remove(booking);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("This booking does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //-----------------------------------------------------------

        public List<BookingInfo> GetBookingInfos()
        {
            var bookingInfos = new List<BookingInfo>();
            try
            {
                using var context = new RentCarDbContext();
                bookingInfos = context.BookingInfos.Include(b => b.RenterAddressId).Include(b => b.DriverAddressId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return bookingInfos;
        }

        public BookingInfo GetBookingInfoById(int? bookingInfoId)
        {
            BookingInfo _bookingInfo = null;
            try
            {
                using var context = new RentCarDbContext();
                _bookingInfo = context.BookingInfos.Include(b => b.RenterAddressId).Include(b => b.DriverAddressId).SingleOrDefault(b => b.BookingInfoId == bookingInfoId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _bookingInfo;
        }

        public void EditBookingInfo(BookingInfo bookingInfo)
        {
            try
            {
                BookingInfo _bookingInfo = GetBookingInfoById(bookingInfo.BookingInfoId);
                if (_bookingInfo != null)
                {
                    using var context = new RentCarDbContext();
                    context.BookingInfos.Update(bookingInfo);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("This BookingInfo does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AddBookingInfo(BookingInfo bookingInfo)
        {
            try
            {
                BookingInfo _bookingInfo = GetBookingInfoById(bookingInfo.BookingInfoId);
                if (_bookingInfo == null)
                {
                    using var context = new RentCarDbContext();
                    context.BookingInfos.Add(bookingInfo);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("This bookingInfo is already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //-----------------------------------------------------------

        public List<Address> GetAddress()
        {
            var address = new List<Address>();
            try
            {
                using var context = new RentCarDbContext();
                address = context.Addresses.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return address;
        }

        public Address GetAddressById(int? addressId)
        {
            Address address = null;
            try
            {
                using var context = new RentCarDbContext();
                address = context.Addresses.Include(a => a.City).Include(a => a.District).Include(a => a.Ward).SingleOrDefault(a => a.AddressId == addressId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return address;
        }

        public void EditAddress(Address address)
        {
            try
            {
                Address _address = GetAddressById(address.AddressId);
                if (_address != null)
                {
                    using var context = new RentCarDbContext();
                    context.Addresses.Update(address);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("This address does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AddAdress(Address address)
        {
            try
            {
                Address _address = GetAddressById(address.AddressId);
                if (_address == null)
                {
                    using var context = new RentCarDbContext();
                    context.Addresses.Add(address);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("This address is already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<City> GetCityList()
        {
            var cities = new List<City>();
            try
            {
                using var context = new RentCarDbContext();
                cities = context.Cities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return cities;
        }

        public List<District> GetDistrictList()
        {
            var districts = new List<District>();
            try
            {
                using var context = new RentCarDbContext();
                districts = context.Districts.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return districts;
        }

        public List<District> GetDistrictListByCity(int? cityId)
        {
            var districts = new List<District>();
            try
            {
                using var context = new RentCarDbContext();
                districts = context.Districts.Where(d => d.CityId == cityId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return districts;
        }

        public List<Ward> GetWardList()
        {
            var wards = new List<Ward>();
            try
            {
                using var context = new RentCarDbContext();
                wards = context.Wards.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return wards;
        }

        public List<Ward> GetWardListByDistrict(int? districtId)
        {
            var wards = new List<Ward>();
            try
            {
                using var context = new RentCarDbContext();
                wards = context.Wards.Where(w => w.DistrictId == districtId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return wards;
        }
    }
}
