using System.ComponentModel.DataAnnotations;

namespace AuctionSite.DAL.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [StringLength(50, ErrorMessage = "Email Must be between 5 and 50 characters", MinimumLength = 5)]
        [EmailAddress(ErrorMessage = "Email Must be a valid email")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(25, ErrorMessage = "Password Must be between 8 and 25 characters", MinimumLength = 8)]
        public string password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(25, ErrorMessage = "Confirm Password Must be between 8 and 25 characters", MinimumLength = 8)]
        [Compare("password", ErrorMessage = "password and password confirmation doesn't match")]
        public string passwordConfirmation { get; set; }
    }
}