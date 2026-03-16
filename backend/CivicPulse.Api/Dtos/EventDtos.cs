using System.ComponentModel.DataAnnotations;
using CivicPulse.Api.Models;

namespace CivicPulse.Api.Dtos;

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Time { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? AiSummary { get; set; }
    public string? WhyItMatters { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public string OrganizerId { get; set; } = string.Empty;
    public bool IsBookmarked { get; set; }
    public bool IsApproved { get; set; }
}

public class EventCreateDto
{
    [Required] public string Title { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
    [Required] public DateTime EventDate { get; set; }
    [Required] public string Time { get; set; } = string.Empty;
    [Required] public string Location { get; set; } = string.Empty;
    [Required] public EventCategory Category { get; set; }
}

public class EventUpdateDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? EventDate { get; set; }
    public string? Time { get; set; }
    public string? Location { get; set; }
    public EventCategory? Category { get; set; }
}

public class EventSearchResultDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? AiSummary { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public double Score { get; set; }
}
