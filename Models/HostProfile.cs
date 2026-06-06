namespace graduation_proj.Models
{
    public class HostProfile
    {
        public int HostProfileId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public string Bio { get; set; }
        public string? PlacePicturePath { get; set; }

        // Foreign key to ApplicationUser
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        // Navigation
        public ICollection<Dish> Dishes { get; set; }
    }
}