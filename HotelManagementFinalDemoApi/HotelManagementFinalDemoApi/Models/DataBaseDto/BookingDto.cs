using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HotelManagementFinalDemoApi.Models.DataModels;

namespace HotelManagementFinalDemoApi.Models.DataBaseDto
{
    public class BookingDto
    {
        [Key]
        public Guid Id { get; set; }


        [Required]
        public Guid UserId { get; set; }
        [Required]
        [ForeignKey("Room")]
       
        public Guid RoomId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }


        [Required]
        public Status Status { get; set; }

        public int ? RoomNo { get; set; }
        public string? HotelName { get; set; }

        public Guid CreatedBy { get; set; }


        public DateTime CreatedDate { get; set; } = DateTime.Now;


        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }


        public static Booking ToEntity(BookingDto bookingDto)
        {
            return new Booking()
            {
                Id=bookingDto.Id,
                UserId=bookingDto.UserId,
                RoomId=bookingDto.RoomId,
                CheckInDate=bookingDto.CheckInDate,
                CheckOutDate=bookingDto.CheckOutDate,
                Status=(int)bookingDto.Status,
                CreatedBy=bookingDto.CreatedBy,
                CreatedDate=bookingDto.CreatedDate,
                UpdatedBy=bookingDto.UpdatedBy,
                UpdatedAt=bookingDto.UpdatedAt

            };
        }
        public static BookingDto FromEntity(Booking booking)
        {
            return new BookingDto()
            {
                Id = booking.Id,
                UserId = booking.UserId,
                RoomId = booking.RoomId,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                Status = (Status)booking.Status,
                RoomNo=booking.Room?.RoomNo,
                HotelName=booking.Room?.Hotel?.HotelName,
                CreatedBy = booking.CreatedBy,
                CreatedDate = booking.CreatedDate,
                UpdatedBy = booking.UpdatedBy,
                UpdatedAt = booking.UpdatedAt
            };
        }
        public static IEnumerable<Booking> ToEntity(IEnumerable<BookingDto> bookingsDtos)
        {
            return bookingsDtos.Select(dto => ToEntity(dto)).ToList();
        }

        public static IEnumerable<BookingDto> FromEntity(IEnumerable<Booking> bookings)
        {
            return bookings.Select(booking => FromEntity(booking)).ToList();
        }
    }
  public  enum Status
    {
        Confirm=1,
        Pending=2,
        cancel=3
    }
}
