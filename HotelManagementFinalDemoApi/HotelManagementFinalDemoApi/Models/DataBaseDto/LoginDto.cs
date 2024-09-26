using System.ComponentModel.DataAnnotations;

namespace HotelManagementFinalDemoApi.Models.DataBaseDto
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
