namespace graduation_proj.Models
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }

    public class Booking
    {
        public int BookingId { get; set; }
        public DateTime VisitDate { get; set; }
        public TimeSpan VisitTime { get; set; }
        public int NumberOfPersons { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // Foreign key to ApplicationUser (the guest)
        public string? GuestId { get; set; }
        public ApplicationUser? Guest { get; set; }

        // Foreign key to Dish
        public int? DishId { get; set; }
        public Dish? Dish { get; set; }
    }
}