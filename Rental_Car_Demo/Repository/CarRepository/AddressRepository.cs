using Microsoft.EntityFrameworkCore;
using Rental_Car_Demo.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rental_Car_Demo.Repository
{
    [ExcludeFromCodeCoverage]
    public class AddressRepository
    {
        public IEnumerable<Address> SearchAddresses(string query)
        {
            using ( var context = new RentCarDbContext () ) {
                var addresses = context.Addresses
                    .Include (a => a.City)
                    .Include (a => a.District)
                    .Include (a => a.Ward)
                     .Where (a => a.District.DistrictName.Contains (query) ||
                            a.Ward.WardName.Contains (query) ||
                            a.City.CityProvince.Contains (query) ||
                            a.HouseNumberStreet.Contains (query))
                .ToList ();
                return addresses;
            }
        }
    }
}
