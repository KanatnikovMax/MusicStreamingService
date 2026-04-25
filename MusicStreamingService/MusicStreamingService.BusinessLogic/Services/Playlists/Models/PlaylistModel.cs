namespace MusicStreamingService.BusinessLogic.Services.Playlists.Models;

public class PlaylistModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? PhotoUrl { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
