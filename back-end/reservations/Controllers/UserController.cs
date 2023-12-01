using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reservations.data;
using reservations.models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Diagnostics;

namespace reservations.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ReservationsDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(ReservationsDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("getUsers")]
        public List<User> GetUsers()
        {
            return _context.GetAllUsers();
        }

        [HttpGet("getUserData/{id}")]
        public User? GetUsers(int id)
        {
            return _context.GetUserById(id);
        }

        [HttpPost("login/{email}/{password}")]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest(new { error = "Both email and password are required" });
            }

            User? userResponse = _context.GetUserByCredentials(email, password);

            if (userResponse == null)
            {
                return Unauthorized(new { error = "Invalid email or password" });
            }
            else
            {
                return Ok(new { message = "Login successful", user = userResponse });
            }
        }

        [HttpPost("register/{name}/{email}/{phoneNumber}/{password}")]
        public IActionResult Register(string name, string email, string phoneNumber, string password)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password))
            {
                return BadRequest(new { error = "Name, email, phone number, and password are required" });
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);

            if (existingUser != null)
            {
                return Conflict(new { error = "This email has already been taken" });
            }

            var newUser = new User(name, email, phoneNumber, password, isAdmin: false);
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new { message = "Login successful" });
        }

        [HttpPut("updateUserData/{id}/{name}/{email}/{phoneNumber}")]
        public IActionResult UpdateUserData(int id, string name, string email, string phoneNumber)
        {
            try
            {
                var existingUser = _context.GetUserById(id);

                if (existingUser == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                existingUser.Name = name;
                existingUser.Email = email;
                existingUser.PhoneNumber = phoneNumber;

                _context.SaveChanges();

                return Ok(new { message = "User data updated successfully", user = existingUser });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating user data", details = ex.Message });
            }
        }
    }
}
