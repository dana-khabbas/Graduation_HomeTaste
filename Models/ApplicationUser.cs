using Microsoft.AspNetCore.Identity;

namespace graduation_proj.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? City { get; set; }

        public bool IsHost { get; set; } = false;

      
        public HostProfile? HostProfile { get; set; }
    }
}