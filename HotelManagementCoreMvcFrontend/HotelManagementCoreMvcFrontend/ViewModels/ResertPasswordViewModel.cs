using System.ComponentModel.DataAnnotations;

namespace HotelManagementCoreMvcFrontend.ViewModels
{
    public class ResertPasswordViewModel
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        [Required(ErrorMessage = "New password is required")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@#$&*])[A-Za-z\\d@#$&*]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, one special character (@,#,$,&,*) and be at least 8 characters long.")]
        public string NewPassword { get; set; }
    }
}
