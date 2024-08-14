using System.ComponentModel.DataAnnotations;

namespace Rental_Car_Demo.ViewModel
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "This field is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }
    }
}
