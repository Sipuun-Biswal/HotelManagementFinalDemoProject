using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementFinalDemoApi.Models.DataModels
{
    public class Hotel
    {
        [Key]
        public Guid? Id { get; set; }

       
        public string HotelName { get; set; }

        [ForeignKey("Country")]
       
        public int CountryId { get; set; }

        [ForeignKey("State")]
       
        public int StateId { get; set; }

        [ForeignKey("City")]
       
        public int CityId { get; set; }
        public Guid? CreatedBy { get; set; }

        [ForeignKey("User")]
       
        public Guid UserId { get; set; }

       public string? Address { get; set; }
        public string? PhoneNo { get; set; }

        public string? HotelImage { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual User User { get; set; }
        
        public virtual Country Country { get; set; }
        public virtual State State { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }

    }
}
