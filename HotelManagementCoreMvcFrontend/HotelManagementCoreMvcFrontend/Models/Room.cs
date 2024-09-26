using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementCoreMvcFrontend.Models
{
    public class Room
    {
        public Guid Id { get; set; }

        [ForeignKey("Hotel")]
        [Required(ErrorMessage ="Hotel Name is required")]
        public Guid HotelId { get; set; }
        public string? HotelName {  get; set; }

        [Required(ErrorMessage ="Room No is required")]
        public int RoomNo { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
