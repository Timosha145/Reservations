using Microsoft.EntityFrameworkCore;
using reservations.models;

namespace reservations.data
{
    public class ReservationsDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Service> Services { get; set; }


        public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : base(options)
        {

        }

        // Users

        public List<User> GetAllUsers()
        {
            return Users.ToList();
        }

        public void AddUser(User user)
        {
            Users.Add(user);
            SaveChanges();
        }

        public User? GetUserById(int id)
        {
            return Users.Find(id);
        }
        
        public User? GetUserByCredentials(string email, string password) 
        {
            return Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        // Reservations

        public List<Reservation> GetAllReservations()
        {
            return Reservations.ToList();
        }

        public void AddReservation(Reservation reservation)
        {
            Reservations.Add(reservation);
            SaveChanges();
        }

        public Reservation? GetReservationById(int id)
        {
            return Reservations.Find(id);
        }

        public void EditReservation(Reservation editedReservation, int editedReservationId)
        {
            var existingReservation = GetReservationById(editedReservationId);

            if (existingReservation != null)
            {
                existingReservation.ClientId = editedReservation.ClientId;
                existingReservation.ServiceId = editedReservation.ServiceId;
                existingReservation.Date = editedReservation.Date;
                existingReservation.Salon = editedReservation.Salon;
                existingReservation.CarNumber = editedReservation.CarNumber;

                SaveChanges();
            }
        }

        public void DeleteReservation(int reservationId)
        {
            var reservationToDelete = GetReservationById(reservationId);

            if (reservationToDelete != null)
            {
                Reservations.Remove(reservationToDelete);
                SaveChanges();
            }
        }


        // Services

        public List<Service> GetAllServices()
        {
            return Services.ToList();
        }

        public void AddService(Service service)
        {
            Services.Add(service);
            SaveChanges();
        }

        public Service? GetServiceById(int id)
        {
            return Services.Find(id);
        }
    }
}
