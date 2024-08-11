using System.ComponentModel.DataAnnotations;

namespace Rental_Car_Demo.ViewModel
{
    public class ResetPassword2ViewModel
    {
        public int CustomerId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{7,}$", ErrorMessage = "Password must be at least seven characters long and contain at least one letter and one number.")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
