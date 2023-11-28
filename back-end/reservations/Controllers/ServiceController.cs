using Microsoft.AspNetCore.Mvc;
using reservations.data;
using reservations.models;

namespace reservations.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ReservationsDbContext _context;

        public ServiceController(ReservationsDbContext context)
        {
            _context = context;
        }

        [HttpGet("getServices")]
        public List<Service> GetServices()
        {
            return _context.GetAllServices();
        }

        [HttpPost("addService/{name}/{price}/{description}/{duration}")]
        public IActionResult AddService(string name, float price, string description, TimeSpan duration)
        {
            try
            {
                var newService = new Service(name, price, description, duration);
                _context.AddService(newService);
                return Ok(new { message = "Service added successfully", service = newService });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while adding the service", details = ex.Message });
            }
        }

        [HttpGet("getServiceById/{id}")]
        public IActionResult GetServiceById(int id)
        {
            var service = _context.GetServiceById(id);
            if (service != null)
            {
                return Ok(service);
            }
            else
            {
                return NotFound("Service not found");
            }
        }
    }
}
