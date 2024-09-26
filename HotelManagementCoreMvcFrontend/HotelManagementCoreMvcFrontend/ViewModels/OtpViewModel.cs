using System.ComponentModel.DataAnnotations;

namespace HotelManagementCoreMvcFrontend.ViewModels
{
    public class OtpViewModel
    {
        [Required(ErrorMessage ="WithOut Code You cant submit")]
     public int Code { get; set; }
        public string? Email { get; set; }
    }
}
