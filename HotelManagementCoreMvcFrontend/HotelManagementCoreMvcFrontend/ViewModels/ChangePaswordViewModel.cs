using System.ComponentModel.DataAnnotations;

namespace HotelManagementCoreMvcFrontend.ViewModels
{
    public class ChangePaswordViewModel
    {
            public Guid UserId { get; set; }

            [Required(ErrorMessage = "Current password is required")]
            [DataType(DataType.Password)]
            public string CurrentPassword { get; set; }

            [Required(ErrorMessage = "New password is required")]
            [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@#$&*])[A-Za-z\\d@#$&*]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, one special character (@,#,$,&,*) and be at least 8 characters long.")]
            public string NewPassword { get; set; }

            [Required(ErrorMessage = "Confirm  password is required")]
            [Compare("NewPassword", ErrorMessage = "New password and confirmation do not match")]
            [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@#$&*])[A-Za-z\\d@#$&*]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, one special character (@,#,$,&,*) and be at least 8 characters long.")]
        public string ConfirmNewPassword { get; set; }
        }

    }

