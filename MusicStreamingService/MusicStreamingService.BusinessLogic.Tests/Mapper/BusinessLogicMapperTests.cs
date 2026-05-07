using AutoFixture;
using FluentAssertions;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.BusinessLogic.Tests.TestHelpers;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Tests.Mapper;

public sealed class BusinessLogicMapperTests
{
    private readonly BusinessLogicTestFixture _test = new();

    private IFixture Fixture => _test.Fixture;

    [Fact]
    public void AlbumMapping_ShouldOrderSongsByTrackNumber()
    {
        var firstSong = Fixture.Build<Song>()
            .With(x => x.Title, "First")
            .With(x => x.TrackNumber, 1)
            .With(x => x.AudioObjectKey, "1")
            .Create();

        var secondSong = Fixture.Build<Song>()
            .With(x => x.Title, "Second")
            .With(x => x.TrackNumber, 2)
            .With(x => x.AudioObjectKey, "2")
            .Create();

        var album = Fixture.Build<Album>()
            .With(x => x.Title, "Album")
            .With(x => x.Songs, [secondSong, firstSong])
            .With(x => x.Artists, [])
            .Create();

        var result = _test.Mapper.Map<AlbumModel>(album);

        result.Songs
            .Select(x => x.TrackNumber)
            .Should()
            .Equal(1, 2);
    }

    [Fact]
    public void ArtistMapping_ShouldOrderAlbumsByReleaseDateDescending()
    {
        var oldAlbum = Fixture.Build<Album>()
            .With(x => x.Title, "Old")
            .With(x => x.ReleaseDate, new DateTime(2020, 1, 1))
            .With(x => x.Artists, [])
            .Create();

        var newAlbum = Fixture.Build<Album>()
            .With(x => x.Title, "New")
            .With(x => x.ReleaseDate, new DateTime(2024, 1, 1))
            .With(x => x.Artists, [])
            .Create();

        var artist = Fixture.Build<Artist>()
            .With(x => x.Name, "Artist")
            .With(x => x.Albums, [oldAlbum, newAlbum])
            .Create();

        var result = _test.Mapper.Map<ArtistModel>(artist);

        result.Albums
            .Select(x => x.Title)
            .Should()
            .Equal("New", "Old");
    }
}