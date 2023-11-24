using Microsoft.AspNetCore.Mvc;
using reservations.data;
using reservations.models;
using System;
using System.Collections.Generic;

namespace reservations.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ReservationsDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReservationController(ReservationsDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("getReservations")]
        public List<Reservation> GetReservations()
        {
            return _context.GetAllReservations();
        }

        [HttpPost("addReservation/{clientId}/{serviceId}/{date}/{salon}/{carNumber}")]
        public IActionResult AddReservation(int clientId, int serviceId, DateTime date, string salon, string carNumber)
        {
            try
            {
                var newReservation = new Reservation(clientId, serviceId, date, salon, carNumber);
                _context.AddReservation(newReservation);
                _context.SaveChanges();

                return Ok(new { message = "Reservation added successfully", reservation = newReservation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while adding the reservation", details = ex.Message });
            }
        }

        [HttpPost("editReservation/{id}/{clientId}/{serviceId}/{date}/{salon}/{carNumber}")]
        public IActionResult EditReservation(int id, int clientId, int serviceId, DateTime date, string salon, string carNumber)
        {
            try
            {
                var editedReservation = new Reservation(clientId, serviceId, date, salon, carNumber);
                _context.EditReservation(editedReservation, id);
                _context.SaveChanges();

                return Ok(new { message = "Reservation edited successfully", reservation = editedReservation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while editing the reservation", details = ex.Message });
            }
        }

        [HttpDelete("deleteReservation/{reservationId}")]
        public IActionResult DeleteReservation(int reservationId)
        {
            try
            {
                var reservationToDelete = _context.GetReservationById(reservationId);

                // Assuming you have validation logic
                if (reservationToDelete == null)
                {
                    return NotFound("Reservation not found");
                }

                _context.DeleteReservation(reservationToDelete.Id);
                _context.SaveChanges();

                return Ok(new { message = "Reservation deleted successfully", reservation = reservationToDelete });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the reservation", details = ex.Message });
            }
        }
    }
}
