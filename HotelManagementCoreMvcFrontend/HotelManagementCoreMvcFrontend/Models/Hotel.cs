using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementCoreMvcFrontend.Models
{
    public class Hotel
    {
        public Guid Id { get; set; } = Guid.Empty;
        [Required(ErrorMessage =" Hotel Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Hotel Name can only contain alphabetic characters.")]
        public string HotelName { get; set; }
        [Required(ErrorMessage ="Country is   required")]
        public int CountryId { get; set; }
        [Required(ErrorMessage = "State is   required")]
        public int StateId { get; set; }
        [Required(ErrorMessage = "City is   required")]
        public int CityId { get; set; }
        [Required(ErrorMessage ="Manager is Required")]
        public Guid? UserId { get; set; }
        public Guid CreatedBy { get; set; } = Guid.Empty;
        [Required(ErrorMessage = "Address Field is required")]
        public string? Address { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        [Required(ErrorMessage ="Phone No is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string? PhoneNo { get; set; }
        public string? HotelImage { get; set; }
        public virtual Country? Country { get; set; }
        public virtual State? State { get; set; }
        public virtual City? City { get; set; }
    }
}