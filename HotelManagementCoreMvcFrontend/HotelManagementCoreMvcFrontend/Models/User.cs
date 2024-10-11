using System.ComponentModel.DataAnnotations;
using System.Data;

namespace HotelManagementCoreMvcFrontend.Models
{
    public class User
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage ="First Name is required")]
        [StringLength(15)]
        public string FirstName { get; set; }


        [Required(ErrorMessage ="Last Name  is required")]
        [StringLength(15)]
        public string LastName { get; set; }


        [Required]
        [EmailAddress]
        [StringLength(100)]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$", ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public string? Password { get; set; }
        [Required]
        public Role Role { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public string? ProfileImage { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Token { get; set; }
    }
    public enum Role
    {
        User=1,
        Admin=2,
        Manager=3
    }
}
