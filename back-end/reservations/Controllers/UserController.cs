using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reservations.data;
using reservations.models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

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

        private User? _loggedInUser
        {
            get
            {
                var userJson = _httpContextAccessor.HttpContext?.Session.GetString("LoggedInUser");
                return userJson != null ? JsonSerializer.Deserialize<User>(userJson) : null;
            }
            set
            {
                var userJson = JsonSerializer.Serialize(value);
                _httpContextAccessor.HttpContext?.Session.SetString("LoggedInUser", userJson);
            }
        }

        [HttpGet("getUsers")]
        public List<User> GetUsers()
        {
            return _context.GetAllUsers();
        }

        [HttpGet("getLoggedInUser")]
        public IActionResult GetLoggedInUser()
        {
            if (_loggedInUser != null)
            {
                return Ok(JsonSerializer.Serialize(_loggedInUser));
            }
            else
            {
                return NotFound();
            }
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

            _loggedInUser = userResponse;

            return Ok(new { message = "Login successful", user = userResponse });
        }

        [HttpPost("register/{name}/{email}/{password}")]
        public IActionResult Register(string name, string email, string password)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest(new { error = "Name, email, and password are required" });
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);

            if (existingUser != null)
            {
                return Conflict(new { error = "This email has already been taken" });
            }

            var newUser = new User(name, email, password, isAdmin: false);
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new { message = "Login successful" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _loggedInUser = null;

            return Ok(new { message = "Logout successful" });
        }
    }
}
