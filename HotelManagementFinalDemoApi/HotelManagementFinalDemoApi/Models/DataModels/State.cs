using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelManagementFinalDemoApi.Models.DataModels
{
    public class State
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Country")]
        
        public int CountryId { get; set; }

        
        public string Name { get; set; }

        // Navigation Properties
       
        public virtual Country? Country { get; set; }
        
        public ICollection<City>? Cities { get; set; }
        
    }
}
