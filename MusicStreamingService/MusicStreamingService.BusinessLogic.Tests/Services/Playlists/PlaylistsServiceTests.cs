using AutoFixture;
using FluentAssertions;
using Moq;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Playlists;
using MusicStreamingService.BusinessLogic.Tests.TestHelpers;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Tests.Services.Playlists;

public sealed class PlaylistsServiceTests
{
    private readonly BusinessLogicTestFixture _test = new();
    private readonly PlaylistsService _sut;

    private IFixture Fixture => _test.Fixture;

    public PlaylistsServiceTests()
    {
        _sut = new PlaylistsService(
            _test.UnitOfWork.Object,
            _test.Mapper,
            _test.MediaStorage.Object);
    }

    [Fact]
    public async Task AddSongAsync_WhenPlaylistBelongsToAnotherUser_ShouldThrowAccessDenied()
    {
        var ownerId = Fixture.Create<Guid>();
        var currentUserId = Fixture.Create<Guid>();
        var playlistId = Fixture.Create<Guid>();
        var songId = Fixture.Create<Guid>();

        var playlist = Fixture.Build<Playlist>()
            .With(x => x.Id, playlistId)
            .With(x => x.UserId, ownerId)
            .Create();

        var song = Fixture.Build<Song>()
            .With(x => x.Id, songId)
            .Create();

        _test.Songs
            .Setup(x => x.FindByIdAsync(songId))
            .ReturnsAsync(song);

        _test.Playlists
            .Setup(x => x.FindByIdAsync(playlistId))
            .ReturnsAsync(playlist);

        var act = () => _sut.AddSongAsync(currentUserId, playlistId, songId);

        await act.Should().ThrowAsync<AccessDeniedException>();

        _test.Playlists.Verify(
            x => x.AddSongAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
            Times.Never);

        _test.UnitOfWork.Verify(x => x.CommitAsync(), Times.Never);
    }
}
