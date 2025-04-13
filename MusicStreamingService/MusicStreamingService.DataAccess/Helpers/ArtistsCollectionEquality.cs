using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.BusinessLogic.Helpers;

public static class ArtistsCollectionEquality
{
    public static bool ArtistsCollectionEquals(ICollection<Artist> artists, ICollection<Artist> otherArtists)
    {
        var otherArtistsSet = new HashSet<Guid>(otherArtists.Select(a => a.Id));
        return otherArtistsSet.SetEquals(artists.Select(a => a.Id));
    }
}