 using HotelManagementFinalDemoApi.Models.DataModels;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementFinalDemoApi.Models.DataBaseDto
{
    public class UserDto
    {
        
        public Guid Id { get; set; }

        [Required]
        [StringLength(15)]
        public string FirstName { get; set; }


        [Required]
        [StringLength(15)]
        public string LastName { get; set; }


        [Required]
        [EmailAddress]
        [StringLength(100)]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$",ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        [Required]

        public string Password { get; set; }
        [Required]
        public Role Role { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public string? ProfileImage { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Token { get; set; }
        public bool IsActive { get; set; } = true;

        // Mapping from UserDto to User
        public static User ToEntity(UserDto userDto)
        {
            return new User()
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = userDto.Password,
                Role = (int)userDto.Role,
                IsEmailVerified = userDto.IsEmailVerified,
                ProfileImage = userDto.ProfileImage,
                CreatedBy = userDto.CreatedBy,
                CreatedAt = userDto.CreatedAt,
                UpdatedBy = userDto.UpdatedBy,
                UpdatedAt = userDto.UpdatedAt,
                Token = userDto.Token,
                IsActive = userDto.IsActive,
            };
        }

        // Mapping from User to UserDto
        public static UserDto FromEntity(User user)
        {
            return new UserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.PasswordHash,
                Role = (Role)user.Role,
                IsEmailVerified = user.IsEmailVerified,
                ProfileImage = user.ProfileImage,
                CreatedBy = user.CreatedBy,
                CreatedAt = user.CreatedAt,
                UpdatedBy = user.UpdatedBy,
                UpdatedAt = user.UpdatedAt,
                Token = user.Token,
                IsActive = user.IsActive,
            };
        }

        // Mapping from IEnumerable<UserDto> to IEnumerable<User>
        public static IEnumerable<User> ToEntity(IEnumerable<UserDto> userDtos)
        {
            return userDtos.Select(dto => ToEntity(dto)).ToList();
        }

        public static IEnumerable<UserDto> FromEntity(IEnumerable<User> users)
        {
            return users.Select(user => FromEntity(user)).ToList();
        }
    }
    public enum Role
    {
        User = 1,
        Admin = 2,
        Manager = 3,


    }
}
