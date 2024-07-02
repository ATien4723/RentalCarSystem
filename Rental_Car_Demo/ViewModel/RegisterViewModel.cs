using System.ComponentModel.DataAnnotations;

namespace Rental_Car_Demo.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "This field is required.")]
        [EmailAddress]
        [MaxLength(50)]
        [RegularExpression(@"^[^@\s]+@gmail\.com$", ErrorMessage = "Email must be a valid gmail.com address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{7,}$", ErrorMessage = "Password must contain at least one number, one numeral, and seven characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm password don’t match. Please try again.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression(@"^0[35789]\d{8}$", ErrorMessage = "Phone number invalid!")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string Role { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public bool AgreeToTerms { get; set; }
    }
}
