namespace graduation_proj.Models
{
    public class Dish
    {
        public int DishId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int ServesPersons { get; set; }
        public string? DishPicturePath { get; set; }

        // Foreign key to HostProfile
        public int HostProfileId { get; set; }
        public HostProfile HostProfile { get; set; }

        // Navigation
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}