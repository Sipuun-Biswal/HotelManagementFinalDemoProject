using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementFinalDemoApi.Models.DataModels
{
    public class Otp
    {
        [Key]
        public int Id { get; set; }

      
        
        public Guid UserId { get; set; }

        
        public int Code { get; set; }

        
        public DateTime ExpTime { get; set; }

        
        
    }
}
