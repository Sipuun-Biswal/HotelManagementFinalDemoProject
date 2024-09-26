using System.ComponentModel.DataAnnotations;
using HotelManagementCoreMvcFrontend.Models;
using Microsoft.AspNetCore.Identity;

namespace HotelManagementCoreMvcFrontend.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name can only contain alphabetic characters.")]
        [StringLength(15)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name can only contain alphabetic characters.")]
        [StringLength(15)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$", ErrorMessage = "Invalid email format")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@#$&*])[A-Za-z\\d@#$&*]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, one special character (@,#,$,&,*) and be at least 8 characters long.")]

        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
    }

