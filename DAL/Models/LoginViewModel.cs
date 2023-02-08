using System.ComponentModel.DataAnnotations;

namespace AuctionSite.DAL.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [StringLength(50, ErrorMessage = "Email Must be between 5 and 50 characters", MinimumLength = 5)]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Email Must be a valid email")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(25, ErrorMessage = "Password Must be between 5 and 25 characters", MinimumLength = 5)]
        public string password { get; set; }
    }
}