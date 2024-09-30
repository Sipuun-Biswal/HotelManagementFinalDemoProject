using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HotelManagementFinalDemoApi.Models.DataModels;

namespace HotelManagementFinalDemoApi.Models.DataBaseDto
{
    public class HotelDto
    {
      
        public Guid? Id { get; set; }

        [Required]
        public string HotelName { get; set; }

        
        [Required]
        public int CountryId { get; set; }

       
        [Required]
        public int StateId { get; set; }

       
        [Required]
        public int CityId { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public Guid? CreatedBy { get; set; }

        public string? Address {  get; set; }
   
        public string? PhoneNo { get; set; }

        public string? HotelImage { get; set; }
        public bool IsActive { get; set; } = true;


        public static Hotel ToEntity(HotelDto hotelDto)
        {
            return new Hotel()
            {
                Id=hotelDto.Id,
                HotelName=hotelDto.HotelName,
                CountryId=hotelDto.CountryId,
                StateId=hotelDto.StateId,
                CityId=hotelDto.CityId,
                UserId=hotelDto.UserId,
                CreatedBy=hotelDto.CreatedBy,
                Address =hotelDto.Address,
                PhoneNo=hotelDto.PhoneNo,
                HotelImage=hotelDto.HotelImage,
                IsActive=hotelDto.IsActive,
            };
        }
        public static HotelDto FromEntity(Hotel hotel)
        {
            return new HotelDto()
            {
                Id = hotel.Id,
                HotelName = hotel.HotelName,
                CountryId = hotel.CountryId,
                StateId = hotel.StateId,
                CityId = hotel.CityId,
                UserId = hotel.UserId,
                CreatedBy=hotel.CreatedBy,
                Address =hotel.Address,
                PhoneNo = hotel.PhoneNo,
                HotelImage = hotel.HotelImage,
                IsActive=hotel.IsActive

            };
        }
        public static IEnumerable<Hotel> ToEntity(IEnumerable<HotelDto> hotelDtos)
        {
            return hotelDtos.Select(dto => ToEntity(dto)).ToList();
        }

        public static IEnumerable<HotelDto> FromEntity(IEnumerable<Hotel> hotels)
        {
            return hotels.Select(hotel => FromEntity(hotel)).ToList();
        }
    }
}
