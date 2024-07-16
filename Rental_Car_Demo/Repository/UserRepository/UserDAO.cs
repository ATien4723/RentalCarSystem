using Microsoft.EntityFrameworkCore;
using Rental_Car_Demo.Models;

namespace Rental_Car_Demo.Repository.UserRepository
{
    public class UserDAO
    {
        private static UserDAO instance = null;
        private static readonly object instanceLock = new object();
        public static UserDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new UserDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<User> GetUserList()
        {
            var users = new List<User>();
            try
            {
                using var context = new RentCarDbContext();
                users = context.Users.Include(u => u.Address).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return users;
        }

        public User GetUserById(int userId)
        {
            User user = null;
            try
            {
                using var context = new RentCarDbContext();
                user = context.Users.Include(u => u.Address).SingleOrDefault(u => u.UserId == userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return user;
        }

        public void Create(User user)
        {
            try
            {
                User _user = GetUserById(user.UserId);
                if (_user == null)
                {
                    using var context = new RentCarDbContext();
                    context.Users.Add(user);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("This user is already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string Edit(User user)
        {
            try
            {
                using var context = new RentCarDbContext();
                User _user = context.Users.Find(user.UserId);
                if (_user == null)
                {
                    return "This user does not already exist.";
                }

                bool emailExists = context.Users.Any(u => u.Email == user.Email && u.UserId != user.UserId);
                if (emailExists)
                {
                    return "Email already existed. Please try another email.";
                }

                // Cập nhật các thuộc tính khác ngoại trừ Password
                _user.Email = user.Email;
                _user.RememberMe = user.RememberMe;
                _user.Role = user.Role;
                _user.Name = user.Name;
                _user.Dob = user.Dob;
                _user.NationalId = user.NationalId;
                _user.Phone = user.Phone;
                _user.AddressId = user.AddressId;
                _user.DrivingLicense = user.DrivingLicense;
                _user.Wallet = user.Wallet;
                _user.Address = user.Address;
                _user.Bookings = user.Bookings;
                _user.Cars = user.Cars;

                // Chỉ cập nhật mật khẩu nếu có mật khẩu mới
                if (!string.IsNullOrEmpty(user.Password))
                {
                    _user.Password = user.Password;
                }

                context.SaveChanges();
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public void Delete(int userId)
        {
            try
            {
                User user = GetUserById(userId);
                if (user != null)
                {
                    using var context = new RentCarDbContext();
                    context.Users.Remove(user);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("This user does not already exist.");
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