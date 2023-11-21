using Microsoft.EntityFrameworkCore;
using reservations.models;

namespace reservations.data
{
    public class ReservationsDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : base(options)
        {

        }

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
    }
}
