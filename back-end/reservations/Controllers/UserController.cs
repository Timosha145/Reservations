using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reservations.data;
using reservations.models;

namespace reservations.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ReservationsDbContext _context;

        public UserController(ReservationsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<User> Get()
        {
            return _context.Users.ToList(); ;
        }

        [HttpPost("login/{email}/{password}")]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest(new { error = "Both email and password are required" });
            }

            var user = _context.GetUserByCredentials(email, password);

            if (user == null)
            {
                return Unauthorized(new { error = "Invalid email or password" });
            }

            var userResponse = new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                isAdmin = user.IsAdmin
            };

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
    }
}
