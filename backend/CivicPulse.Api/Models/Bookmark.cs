namespace CivicPulse.Api.Models;

public class Bookmark
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
