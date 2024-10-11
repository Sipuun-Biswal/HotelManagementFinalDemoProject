using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HotelManagementFinalDemoApi.Models.DataModels;

namespace HotelManagementFinalDemoApi.Models.DataBaseDto
{
    public class FeedbackDto
    {
        [Key]
        public Guid Id { get; set; }


        [Required]
        public Guid BookingId { get; set; }

        public DateTime? CheckoutTime { get; set; }
        public string? HotelName { get; set; }
        public int? RoomNo { get; set; }

        [Required]
        public string FeedbackText { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }


        public static Feedback ToEntity(FeedbackDto feedbackDto)
        {
            return new Feedback() { Id = feedbackDto.Id,
                                    BookingId=feedbackDto.BookingId,
                                     FeedbackText=feedbackDto.FeedbackText,
                                      Rating=feedbackDto.Rating };
        }
        public static FeedbackDto FromEntity(Feedback feedback)
        {
            return new FeedbackDto() {

                Id = feedback.Id,
                BookingId = feedback.BookingId,
               CheckoutTime = feedback.Booking?.CheckOutDate,
                HotelName =feedback.Booking?.Room?.Hotel?.HotelName,
                RoomNo=feedback.Booking?.Room?.RoomNo,
                FeedbackText = feedback.FeedbackText,
                Rating = feedback.Rating


            };
        }

        public static IEnumerable<Feedback> ToEntity(IEnumerable<FeedbackDto> feedbackDtos)
        {
            return feedbackDtos.Select(dto => ToEntity(dto)).ToList();
        }

        public static IEnumerable<FeedbackDto> FromEntity(IEnumerable<Feedback> feedbacks)
        {
            return feedbacks.Select(feedback => FromEntity(feedback)).ToList();
        }
    }
}
