namespace HotelManagementFinalDemoApi.Helpers
{
    using HotelManagementFinalDemoApi.Models.DataBaseDto;
    using HotelManagementFinalDemoApi.Models.DataModels;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public class TokenHelper
    {
        private readonly string _jwtSecret;
        private readonly int _jwtLifespan;

        
        public TokenHelper(IConfiguration configuration)
        {
            _jwtSecret = configuration["Jwt:Key"];  
            _jwtLifespan = int.Parse(configuration["Jwt:Lifespan"]);  
        }

       
        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, ((Role)user.Role).ToString()),
            new Claim(ClaimTypes.Name,user.FirstName + " " +user.LastName),
            new Claim("Images",user.ProfileImage!=null?user.ProfileImage:"")
        };

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7119/",
                audience: "https://localhost:7119/",
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtLifespan),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    }

