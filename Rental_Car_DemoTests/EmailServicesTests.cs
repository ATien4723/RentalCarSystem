//using Moq;
//using NUnit.Framework;
//using System.Net.Mail;
//using Rental_Car_Demo.Services;

//namespace Rental_Car_Demo.Tests.Services
//{
//    [TestFixture]
//    public class EmailServiceTests
//    {
//        private Mock<SmtpClient> _mockSmtpClient;
//        private EmailService _emailService;

//        [SetUp]
//        public void SetUp()
//        {
//            _mockSmtpClient = new Mock<SmtpClient>();
//            _emailService = new EmailService(_mockSmtpClient.Object);
//        }

//        [Test]
//        public void SendEmail_ShouldSendEmailWithCorrectParameters()
//        {
//            // Arrange
//            var email = "test@example.com";
//            var subject = "Test Subject";
//            var message = "Test Message";
//            MailMessage capturedMailMessage = null;

//            _mockSmtpClient.Setup(c => c.Send(It.IsAny<MailMessage>()))
//                .Callback<MailMessage>(msg => capturedMailMessage = msg);

//            // Act
//            _emailService.SendEmail(email, subject, message);

//            // Assert
//            Assert.NotNull(capturedMailMessage);
//            Assert.AreEqual("kietnvt2705@gmail.com", capturedMailMessage.From.Address);
//            Assert.AreEqual(email, capturedMailMessage.To[0].Address);
//            Assert.AreEqual(subject, capturedMailMessage.Subject);
//            Assert.AreEqual(message, capturedMailMessage.Body);
//        }
//    }
//}