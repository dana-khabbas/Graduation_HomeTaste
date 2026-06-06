namespace graduation_proj.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key to ApplicationUser (the guest)
        public string? GuestId { get; set; }
        public ApplicationUser? Guest { get; set; }

        // Foreign key to Dish
        public int? DishId { get; set; }
        public Dish? Dish { get; set; }
    }
}