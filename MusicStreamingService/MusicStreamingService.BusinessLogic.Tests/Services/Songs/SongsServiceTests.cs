using AutoFixture;
using FluentAssertions;
using Moq;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Media.Models;
using MusicStreamingService.BusinessLogic.Services.Songs;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.BusinessLogic.Tests.TestHelpers;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Tests.Services.Songs;

public sealed class SongsServiceTests
{
    private readonly BusinessLogicTestFixture _test = new();
    private readonly SongsService _sut;

    private IFixture Fixture => _test.Fixture;

    public SongsServiceTests()
    {
        _sut = new SongsService(
            _test.UnitOfWork.Object,
            _test.Mapper,
            _test.Cache.Object,
            _test.MediaStorage.Object);
    }

    [Fact]
    public async Task CreateSongAsync_WhenAlbumAndArtistsAreValid_ShouldSaveUploadAudioCommitAndReturnSong()
    {
        var albumArtist = Fixture.Build<Artist>()
            .With(x => x.Name, "Album Artist")
            .Create();

        var album = Fixture.Build<Album>()
            .With(x => x.Artists, [albumArtist])
            .Create();

        var model = Fixture.Build<CreateSongModel>()
            .With(x => x.AlbumId, album.Id)
            .With(x => x.Artists, ["Album Artist"])
            .Create();

        var songArtists = new List<Artist> { albumArtist };
        const string audioObjectKey = "songs/song.mp3";

        _test.Albums
            .Setup(x => x.FindByIdAsync(album.Id))
            .ReturnsAsync(album);

        _test.Artists
            .Setup(x => x.GetOrCreateArtistsAsync(model.Artists))
            .ReturnsAsync(songArtists);

        _test.Songs
            .Setup(x => x.SaveAsync(It.IsAny<Song>()))
            .ReturnsAsync((Song song) => song);

        _test.MediaStorage
            .Setup(x => x.UploadAsync(
                It.Is<FileUploadModel>(file =>
                    file.FileName.EndsWith(".mp3") &&
                    file.ContentType == "audio/mpeg"),
                "songs",
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(audioObjectKey);

        var result = await _sut.CreateSongAsync(model, [1, 2, 3]);

        result.Title.Should().Be(model.Title);
        result.Duration.Should().Be(model.Duration);
        result.TrackNumber.Should().Be(model.TrackNumber);
        result.AlbumId.Should().Be(album.Id);
        result.AudioObjectKey.Should().Be(audioObjectKey);
        result.Artists.Should().ContainSingle(x => x.Name == albumArtist.Name);

        _test.Songs.Verify(
            x => x.SaveAsync(It.Is<Song>(song =>
                song.Title == model.Title &&
                song.Album == album &&
                song.Artists == songArtists)),
            Times.Once);

        _test.MediaStorage.Verify(
            x => x.UploadAsync(
                It.IsAny<FileUploadModel?>(),
                "songs",
                It.Is<Guid>(id => id == result.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _test.UnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        _test.UnitOfWork.Verify(x => x.RollbackAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateSongAsync_WhenAlbumDoesNotExist_ShouldRollbackAndThrow()
    {
        var model = Fixture.Build<CreateSongModel>()
            .With(x => x.Artists, ["Artist"])
            .Create();

        _test.Albums
            .Setup(x => x.FindByIdAsync(model.AlbumId))
            .ReturnsAsync((Album?)null);

        var act = () => _sut.CreateSongAsync(model, [1, 2, 3]);

        await act.Should().ThrowAsync<EntityNotFoundException>();

        _test.UnitOfWork.Verify(x => x.RollbackAsync(), Times.AtLeastOnce);

        _test.Songs.Verify(
            x => x.SaveAsync(It.IsAny<Song>()),
            Times.Never);

        _test.MediaStorage.Verify(
            x => x.UploadAsync(
                It.IsAny<FileUploadModel?>(),
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateSongAsync_WhenSongArtistIsNotAlbumArtist_ShouldRollbackAndThrow()
    {
        var albumArtist = Fixture.Build<Artist>()
            .With(x => x.Name, "Album Artist")
            .Create();

        var album = Fixture.Build<Album>()
            .With(x => x.Artists, [albumArtist])
            .Create();

        var model = Fixture.Build<CreateSongModel>()
            .With(x => x.AlbumId, album.Id)
            .With(x => x.Artists, ["Other Artist"])
            .Create();

        _test.Albums
            .Setup(x => x.FindByIdAsync(album.Id))
            .ReturnsAsync(album);

        var act = () => _sut.CreateSongAsync(model, [1, 2, 3]);

        await act.Should().ThrowAsync<WrongArtistNameConsistencyException>();

        _test.UnitOfWork.Verify(x => x.RollbackAsync(), Times.AtLeastOnce);

        _test.Songs.Verify(
            x => x.SaveAsync(It.IsAny<Song>()),
            Times.Never);
    }

    [Fact]
    public async Task RecordSongPlayedAsync_WhenSongExists_ShouldIncrementCommitAndInvalidateSongAndArtists()
    {
        var songId = Fixture.Create<Guid>();
        var artistIds = Fixture.CreateMany<Guid>(2).ToList();

        _test.Songs
            .Setup(x => x.IncrementPlayCountAsync(songId))
            .ReturnsAsync(artistIds);

        await _sut.RecordSongPlayedAsync(songId);

        _test.Songs.Verify(
            x => x.IncrementPlayCountAsync(songId),
            Times.Once);

        _test.UnitOfWork.Verify(
            x => x.CommitAsync(),
            Times.Once);

        _test.Cache.Verify(
            x => x.RemoveAsync($"songs_{songId}", It.IsAny<CancellationToken>()),
            Times.Once);

        foreach (var artistId in artistIds)
        {
            _test.Cache.Verify(
                x => x.RemoveAsync($"artists_{artistId}", It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    [Fact]
    public async Task GetSongAudioUrlAsync_WhenSongHasNoAudioObjectKey_ShouldThrow()
    {
        var songId = Fixture.Create<Guid>();

        var song = Fixture.Build<Song>()
            .With(x => x.Id, songId)
            .With(x => x.AudioObjectKey, string.Empty)
            .Create();

        _test.Songs
            .Setup(x => x.FindByIdAsync(songId))
            .ReturnsAsync(song);

        var act = () => _sut.GetSongAudioUrlAsync(songId);

        await act.Should().ThrowAsync<EntityNotFoundException>();

        _test.MediaStorage.Verify(
            x => x.GetReadUrlAsync(
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
