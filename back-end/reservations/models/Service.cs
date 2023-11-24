namespace reservations.models
{
    public class Service
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public float Price { get; set; }
        public string? Description { get; set; }
        public TimeSpan Duration { get; set; }

        private Service() { }

        public Service (string? name, float price, string? description, TimeSpan duration)
        {
            Name = name;
            Price = price;
            Description = description;
            Duration = duration;
        }
    }
}
