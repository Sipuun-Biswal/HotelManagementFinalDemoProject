using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementFinalDemoApi.Models.DataModels
{
    public class ResertPassword
    {
        [Key]
       public int Id { get; set; }
       public Guid UserId { get; set; }
       public string? Token { get; set; } 
       public DateTime ExiparyTime { get; set; }
      
    }
}
