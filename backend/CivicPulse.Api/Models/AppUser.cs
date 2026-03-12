using Microsoft.AspNetCore.Identity;

namespace CivicPulse.Api.Models;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();
    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
}
