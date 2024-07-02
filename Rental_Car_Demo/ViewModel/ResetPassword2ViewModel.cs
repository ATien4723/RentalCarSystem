using System.ComponentModel.DataAnnotations;

namespace Rental_Car_Demo.ViewModel
{
    public class ResetPassword2ViewModel
    {
        public int CustomerId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
