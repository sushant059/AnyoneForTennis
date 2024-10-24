using System.ComponentModel.DataAnnotations;

namespace AnyoneForTennis.ViewModels
{
    public class RegisterCoachViewModel
    {
        [Required(ErrorMessage = "The First Name field is required.")]
        [StringLength(50, ErrorMessage = "The First name must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The Last Name field is required.")]
        [StringLength(50, ErrorMessage = "The Last name must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }

        public string? Biography { get; set; }

        public byte[]? Photo { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [DataType(DataType.EmailAddress,ErrorMessage ="Enter Valid Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
