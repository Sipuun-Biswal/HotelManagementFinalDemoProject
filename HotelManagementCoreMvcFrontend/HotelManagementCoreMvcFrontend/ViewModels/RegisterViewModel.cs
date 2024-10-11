using System.ComponentModel.DataAnnotations;
using HotelManagementCoreMvcFrontend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HotelManagementCoreMvcFrontend.Helper;

namespace HotelManagementCoreMvcFrontend.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name can only contain alphabetic characters.")]
        [StringLength(15)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name can only contain alphabetic characters.")]
        [StringLength(15)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$", ErrorMessage = "Invalid email format")]
        [Remote(action: "IsEmailAvailable", controller: "Authentication", ErrorMessage = "This mail already exist ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@#$&*])[A-Za-z\\d@#$&*]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, one special character (@,#,$,&,*) and be at least 8 characters long.")]

        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
        [AllowedFileExtensions(new[] { ".jpg", ".jpeg", ".png", ".gif" }, ErrorMessage = "Please upload a valid image file (jpg, jpeg, png, gif).")]
        public IFormFile? ProfileImage { get; set; }
        [Required(ErrorMessage ="Role is required")]
        public Role Role { get; set; }
    }
 
}

