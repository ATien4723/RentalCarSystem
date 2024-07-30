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
        private UsersController _usersController;
        private Mock<RentCarDbContext> _dbContextMock;

        [SetUp]
        public void Setup()
        {
            // Tạo mock cho RentCarDbContext
            _dbContextMock = new Mock<RentCarDbContext>();

            // Tạo instance của UsersController với RentCarDbContext mock
            _usersController = new UsersController(_dbContextMock.Object);
        }

        [Test, MaxTime(2000)]
        [TestCase("duyquan7b@gmail.com", true)]
        [TestCase("duyquan7b1@gmail.com", true)]
        [TestCase("hfa@gmail.com", false)]
        [TestCase("hfa.com", false)]
        [TestCase("minhquandancom", false)]
        public void IsEmailExist_Return(string email, bool expected)
        {
            bool result = uc.IsEmailExist(email);
            Assert.That(result, Is.EqualTo(expected));
        }




    }
}
