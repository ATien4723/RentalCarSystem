using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Rental_Car_Demo.Context;
using Rental_Car_Demo.Models;
using System.Collections.Generic;
using System.Linq;

namespace Rental_Car_Demo.Tests.Context
{
    [TestFixture]
    public class CustomerContextTestsKiet
    {
        private Mock<RentCarDbContext> _mockContext;
        private CustomerContext _customerContext;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<RentCarDbContext>();
            _customerContext = new CustomerContext();
            _customerContext.context = _mockContext.Object;
        }

        [Test]
        public void GetCustomerIdByEmail_ShouldReturnCustomerId_WhenCustomerExists()
        {
            // Arrange
            var email = "test@example.com";
            var users = new List<User>
            {
                new User { UserId = 1, Email = email }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            // Act
            var result = _customerContext.getCustomerIdByEmail(email);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetCustomerIdByEmail_ShouldReturnMinusOne_WhenCustomerDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var users = new List<User>().AsQueryable();

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            // Act
            var result = _customerContext.getCustomerIdByEmail(email);

            // Assert
            Assert.AreEqual(-1, result);
        }
    }
}
