using System.ComponentModel.DataAnnotations;

namespace HotelManagementCoreMvcFrontend.ViewModels
{
    public class OtpViewModel
    {
      public string? Email { get; set; }
    [Required(ErrorMessage ="Code is required")]
    public string Code { get; set; }
        
    }
}
