using Microsoft.AspNetCore.Mvc;
using Moq;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rental_Car_Demo.UnitTests
{
    [TestFixture]

    public class UsersControllerTests
    {
        private UsersController _uc;
        private RentCarDbContext _db;

        [SetUp]
        public void Setup()
        {
            // Tạo mock cho RentCarDbContext
            _db = new RentCarDbContext();

            // Tạo instance của UsersController với RentCarDbContext mock
            _uc = new UsersController();
            
        }

        
        [TearDown]
        public void TearDown()
        {
            _uc.Dispose();
            _db.Dispose();

        }

        [Test, MaxTime(2000)]
        [TestCase("duyquan7b@gmail.com", true)]
        [TestCase("duyquan7b1@gmail.com", true)]
        [TestCase("hfa@gmail.com", false)]
        [TestCase("hfa.com", false)]
        [TestCase("minhquandancom", false)]
        public void IsEmailExist_Return(string email, bool expected)
        {
            bool result = _uc.IsEmailExist(email);
            Assert.That(result, Is.EqualTo(expected));
        }




    }
}
