using System.ComponentModel.DataAnnotations;

namespace Rental_Car_Demo.ViewModel
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
