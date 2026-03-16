namespace CivicPulse.Api.Models;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Time { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public EventCategory Category { get; set; }
    public string? AiSummary { get; set; }
    public string? WhyItMatters { get; set; }
    public bool IsApproved { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public string OrganizerId { get; set; } = string.Empty;
    public AppUser Organizer { get; set; } = null!;

    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
}
