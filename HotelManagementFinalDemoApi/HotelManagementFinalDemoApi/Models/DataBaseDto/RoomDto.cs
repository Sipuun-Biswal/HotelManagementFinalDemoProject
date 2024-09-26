using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HotelManagementFinalDemoApi.Models.DataModels;

namespace HotelManagementFinalDemoApi.Models.DataBaseDto
{
    public class RoomDto
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Hotel")]
        [Required]
        public Guid HotelId { get; set; }
        public string? HotelName { get; set; }

        [Required]
        public int RoomNo { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;


        public static Room ToEntity(RoomDto roomDto)
        {
            return new Room()
            {
                Id=roomDto.Id,
                HotelId=roomDto.HotelId,
                RoomNo=roomDto.RoomNo,
                Price=roomDto.Price,
                IsAvailable=roomDto.IsAvailable
            };
        }

        public static RoomDto FromEntity(Room room)
        {
            return new RoomDto()
            {
                Id = room.Id,
                HotelId = room.HotelId,
                HotelName=room.Hotel?.HotelName,
                RoomNo = room.RoomNo,
                Price = room.Price,
                IsAvailable = room.IsAvailable
            };
        }
        public static IEnumerable<Room> ToEntity(IEnumerable<RoomDto> roomDtos)
        {
            return roomDtos.Select(dto => ToEntity(dto)).ToList();
        }

        public static IEnumerable<RoomDto> FromEntity(IEnumerable<Room> rooms)
        {
            return rooms.Select(room => FromEntity(room)).ToList();
        }
    }
}
