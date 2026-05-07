using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Songs;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.Infrastructure.Kafka.ListeningHistory;
using MusicStreamingService.Service.Controllers;
using MusicStreamingService.Service.Controllers.Requests.Songs;
using MusicStreamingService.Service.Mapper;

namespace MusicStreamingService.Service.Tests.Controllers;

public sealed class SongsControllerTests
{
    private readonly Mock<ISongsService> _songsService = new();
    private readonly Mock<IListeningHistoryProducer> _producer = new();
    private readonly SongsController _sut;

    public SongsControllerTests()
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<SongsServiceProfile>()).CreateMapper();
        _sut = new SongsController(
            _songsService.Object,
            _producer.Object,
            mapper,
            Mock.Of<ILogger<SongsController>>());
    }

    [Fact]
    public async Task UploadSong_WhenAudioFileIsEmpty_ShouldReturnBadRequest()
    {
        var request = CreateRequest(CreateFormFile([], "audio/mpeg"));

        var result = await _sut.UploadSong(request);

        var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Audio file is required");
        _songsService.Verify(x => x.CreateSongAsync(It.IsAny<CreateSongModel>(), It.IsAny<byte[]>()), Times.Never);
    }

    [Fact]
    public async Task UploadSong_WhenAudioFileIsNotMp3_ShouldReturnBadRequest()
    {
        var request = CreateRequest(CreateFormFile([1, 2, 3], "audio/wav"));

        var result = await _sut.UploadSong(request);

        var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Only MP3 files are allowed");
        _songsService.Verify(x => x.CreateSongAsync(It.IsAny<CreateSongModel>(), It.IsAny<byte[]>()), Times.Never);
    }

    [Fact]
    public async Task SongPlayed_WhenCurrentUserDoesNotMatchRouteUser_ShouldThrowAccessDenied()
    {
        var routeUserId = Guid.NewGuid();
        SetCurrentUser(Guid.NewGuid());

        var act = () => _sut.SongPlayed(routeUserId, Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<AccessDeniedException>();
        _songsService.Verify(x => x.RecordSongPlayedAsync(It.IsAny<Guid>()), Times.Never);
        _producer.Verify(
            x => x.ProduceSongPlayedAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SongPlayed_WhenCurrentUserMatchesRouteUser_ShouldRecordAndProduceEvent()
    {
        var userId = Guid.NewGuid();
        var songId = Guid.NewGuid();
        SetCurrentUser(userId);

        var result = await _sut.SongPlayed(userId, songId, CancellationToken.None);

        result.Should().BeOfType<AcceptedResult>();
        _songsService.Verify(x => x.RecordSongPlayedAsync(songId), Times.Once);
        _producer.Verify(
            x => x.ProduceSongPlayedAsync(userId, songId, It.IsAny<DateTime>(), CancellationToken.None),
            Times.Once);
    }

    private static CreateSongRequest CreateRequest(IFormFile audioFile)
    {
        return new CreateSongRequest("Track", 180, 1, Guid.NewGuid(), ["Artist"], audioFile);
    }

    private static IFormFile CreateFormFile(byte[] content, string contentType)
    {
        var stream = new MemoryStream(content);
        return new FormFile(stream, 0, content.Length, "AudioFile", "track.mp3")
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    private void SetCurrentUser(Guid userId)
    {
        var identity = new ClaimsIdentity([new Claim("sub", userId.ToString())]);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
    }
}
