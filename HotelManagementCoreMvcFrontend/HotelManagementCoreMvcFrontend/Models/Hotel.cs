using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementCoreMvcFrontend.Models
{
    public class Hotel
    {
        public Guid Id { get; set; } = Guid.Empty;
        [Required(ErrorMessage ="Without Hotel name you cant create")]
        public string HotelName { get; set; }
        [Required(ErrorMessage ="Country is   required")]
        public int CountryId { get; set; }
        [Required(ErrorMessage = "State is   required")]
        public int StateId { get; set; }
        [Required(ErrorMessage = "City is   required")]
        public int CityId { get; set; }
        public Guid? UserId { get; set; }
        [Required(ErrorMessage = "Address Field is required")]
        public Guid CreatedBy { get; set; } = Guid.Empty;
        public string? Address { get; set; }
        [Required(ErrorMessage ="Phone number is required")]
        public string? PhoneNo { get; set; }
        public string? HotelImage { get; set; }
        public virtual Country? Country { get; set; }
        public virtual State? State { get; set; }
        public virtual City? City { get; set; }
    }
}