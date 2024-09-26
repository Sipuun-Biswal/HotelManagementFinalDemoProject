using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementCoreMvcFrontend.Models
{
    public class Hotel
    {
        public Guid? Id { get; set; }
        public string HotelName { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public Guid? UserId { get; set; }
        public string? Address { get; set; }
        public string? PhoneNo { get; set; }
        public string? HotelImage { get; set; }
        public virtual Country? Country { get; set; }
        public virtual State? State { get; set; }
        public virtual City? City { get; set; }
    }
}