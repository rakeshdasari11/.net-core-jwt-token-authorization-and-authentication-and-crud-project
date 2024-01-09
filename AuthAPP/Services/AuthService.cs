using AuthAPP.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using AuthAPP.Data;

namespace AuthAPP.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ApiDbContext _context;

        public AuthService(IConfiguration configuration, ApiDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public User RegisterUser(User model)
        {
            if (_context.Users.Any(u => u.Username == model.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }
            //model.Role = "User";
            _context.Users.Add(model);
            _context.SaveChanges();
            return model; // Return the registered user
        }

        public User RegisterAdmin(User model)
        {
           
            if (_context.Users.Any(u => u.Username == model.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }

            // Add the admin to the database
            model.Role = "Admin"; // Set the role for the admin
            _context.Users.Add(model);
            _context.SaveChanges();

            return model; // Return the registered admin
        }

        public User LoginUser(UserLogin model)
        {

           var user = _context.Users.FirstOrDefault(u => u.Username == model.Username && u.PasswordHash == model.Password);

            return user; 
        }
    }
}