using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelManagementFinalDemoApi.Models.DataModels
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        
        public string Name { get; set; }

        // Navigation Properties
       
        public virtual ICollection<State>? States { get; set; }
        
    }
}
