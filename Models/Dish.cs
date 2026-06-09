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

        // How many times this dish was booked (stored as its own database column).
        // It starts at 0 and goes up by 1 each time a guest books this dish.
        public int TimesBooked { get; set; } = 0;

        // New dishes start as false until the admin approves them.
        public bool IsApproved { get; set; } = false;

        // Foreign key to HostProfile
        public int HostProfileId { get; set; }
        public HostProfile HostProfile { get; set; }

        // Navigation
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}