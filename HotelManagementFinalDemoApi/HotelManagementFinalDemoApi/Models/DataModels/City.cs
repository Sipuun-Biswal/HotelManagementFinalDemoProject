using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelManagementFinalDemoApi.Models.DataModels
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("State")]
       
        public int StateId { get; set; }

       
        public string Name { get; set; }
        //Navigation  Properties
        
        public virtual State? State { get; set; }
       
    }
}
