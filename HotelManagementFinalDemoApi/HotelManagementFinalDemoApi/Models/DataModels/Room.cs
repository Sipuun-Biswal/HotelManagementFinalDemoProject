using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementFinalDemoApi.Models.DataModels
{
    public class Room
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Hotel")]
        
        public Guid HotelId { get; set; }

        
        public int RoomNo { get; set; }

   
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;


        public virtual Hotel Hotel { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
