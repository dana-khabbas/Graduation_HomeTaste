using Microsoft.AspNetCore.Identity;

namespace graduation_proj.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? City { get; set; }

        public bool IsHost { get; set; } = false;

        // When the user created their account
        public DateTime JoinedAt { get; set; } = DateTime.Now;

        public HostProfile? HostProfile { get; set; }
    }
}