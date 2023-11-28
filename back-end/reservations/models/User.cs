namespace reservations.models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public bool IsAdmin { get; set; }

        private User() { }

        public User(string name, string email, string phoneNumber, string password, bool isAdmin) 
        { 
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            IsAdmin = isAdmin;
        }
    }
}
