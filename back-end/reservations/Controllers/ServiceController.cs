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
        public IActionResult AddService(string name, float price, string description, string duration)
        {
            try
            {
                string decodedTimeString = Uri.UnescapeDataString(duration.Replace("%3A", ":"));
                TimeSpan timeSpan = TimeSpan.Parse(decodedTimeString);

                var newService = new Service(name, price, description, timeSpan);
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

        [HttpDelete("deleteService/{id}")]
        public IActionResult DeleteService(int id)
        {
            try
            {
                var serviceToDelete = _context.GetServiceById(id);
                if (serviceToDelete != null)
                {
                    _context?.DeleteService(id);
                    return Ok(new { message = "Service deleted successfully" });
                }
                else
                {
                    return NotFound("Service not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the service", details = ex.Message });
            }
        }

        [HttpPut("updateService/{id}/{name}/{price}/{description}/{duration}")]
        public IActionResult UpdateService(int id, string name, float price, string description, string duration)
        {
            try
            {
                string decodedTimeString = Uri.UnescapeDataString(duration.Replace("%3A", ":"));
                TimeSpan timeSpan = TimeSpan.Parse(decodedTimeString);

                var existingService = _context.GetServiceById(id);
                if (existingService != null)
                {
                    existingService.Name = name;
                    existingService.Price = price;
                    existingService.Description = description;
                    existingService.Duration = timeSpan;

                    _context?.EditService(existingService, id);
                    return Ok(new { message = "Service updated successfully", service = existingService });
                }
                else
                {
                    return NotFound("Service not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the service", details = ex.Message });
            }
        }
    }
}
