using System.ComponentModel.DataAnnotations;

namespace Rental_Car_Demo.ViewModel
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(50)]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{7,}$", ErrorMessage = "Password must be at least seven characters long and contain at least one letter and one number.")]
        public string Password { get; set; }


        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm password don’t match. Please try again.")]
        public string ConfirmPassword { get; set; }

        

        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression(@"^0[35789]\d{8}$", ErrorMessage = "Phone number invalid!")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string Role { get; set; }


        [Required(ErrorMessage = "This field is required.")]
        public bool AgreeToTerms { get; set; }
    }
}
