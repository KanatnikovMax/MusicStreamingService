using MusicStreamingService.BusinessLogic.Services.Songs.Models;

namespace MusicStreamingService.BusinessLogic.Services.Users.Models;

public class ListeningHistoryItemModel
{
    public Guid EventId { get; set; }
    public DateTime ListenedAtUtc { get; set; }
    public SongModel Song { get; set; }
}
