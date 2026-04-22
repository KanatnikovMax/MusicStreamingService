using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Playlists;
using MusicStreamingService.BusinessLogic.Services.Playlists.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.Service.Controllers.Requests.Pagination;
using MusicStreamingService.Service.Controllers.Requests.Playlists;
using MusicStreamingService.Service.Controllers.Responses.Pagination;

namespace MusicStreamingService.Service.Controllers;

[ApiController]
[Authorize]
[Route("users/{userId:guid}/playlists")]
public class PlaylistsController : ControllerBase
{
    private readonly IPlaylistsService _playlistsService;
    private readonly IMapper _mapper;

    public PlaylistsController(IPlaylistsService playlistsService, IMapper mapper)
    {
        _playlistsService = playlistsService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<PlaylistModel>> Create(Guid userId, [FromForm] CreatePlaylistRequest request,
        [FromForm] IFormFile? photo)
    {
        EnsureCurrentUser(userId);
        var model = _mapper.Map<CreatePlaylistModel>(request);
        if (photo != null)
        {
            using var memoryStream = new MemoryStream();
            await photo.CopyToAsync(memoryStream);
            model.Photo = memoryStream.ToArray();
        }
        return await _playlistsService.CreateAsync(userId, model);
    }

    [HttpPatch]
    [Route("{playlistId:guid}")]
    public async Task<ActionResult<PlaylistModel>> Update(Guid userId, Guid playlistId,
        [FromForm] UpdatePlaylistRequest request, [FromForm] IFormFile? photo)
    {
        EnsureCurrentUser(userId);
        var model = _mapper.Map<UpdatePlaylistModel>(request);
        if (photo != null)
        {
            using var memoryStream = new MemoryStream();
            await photo.CopyToAsync(memoryStream);
            model.Photo = memoryStream.ToArray();
        }
        return await _playlistsService.UpdateAsync(userId, playlistId, model);
    }

    [HttpDelete]
    [Route("{playlistId:guid}")]
    public async Task<ActionResult<PlaylistModel>> Delete(Guid userId, Guid playlistId)
    {
        EnsureCurrentUser(userId);
        return await _playlistsService.DeleteAsync(userId, playlistId);
    }

    [HttpGet]
    public async Task<PaginatedResponse<DateTime?, PlaylistModel>> GetUserPlaylists(Guid userId,
        [FromQuery] string? namePart, [FromQuery] PaginationRequest<DateTime?> request)
    {
        EnsureCurrentUser(userId);
        var paginationParams = _mapper.Map<PaginationParams<DateTime?>>(request);
        var playlists = await _playlistsService.GetUserPlaylistsAsync(userId, namePart, paginationParams);
        return _mapper.Map<PaginatedResponse<DateTime?, PlaylistModel>>(playlists);
    }

    [HttpGet]
    [Route("{playlistId:guid}/songs")]
    public async Task<PaginatedResponse<int?, SongModel>> GetPlaylistSongs(Guid userId, Guid playlistId,
        [FromQuery] string? namePart, [FromQuery] PaginationRequest<int?> request)
    {
        EnsureCurrentUser(userId);
        var paginationParams = _mapper.Map<PaginationParams<int?>>(request);
        var songs = await _playlistsService.GetPlaylistSongsAsync(userId, playlistId, namePart, paginationParams);
        return _mapper.Map<PaginatedResponse<int?, SongModel>>(songs);
    }

    [HttpPost]
    [Route("{playlistId:guid}/songs/{songId:guid}")]
    public async Task<ActionResult<PlaylistSongModel>> AddSong(Guid userId, Guid playlistId, Guid songId)
    {
        EnsureCurrentUser(userId);
        return await _playlistsService.AddSongAsync(userId, playlistId, songId);
    }

    [HttpDelete]
    [Route("{playlistId:guid}/songs/{songId:guid}")]
    public async Task<ActionResult<PlaylistSongModel>> RemoveSong(Guid userId, Guid playlistId, Guid songId)
    {
        EnsureCurrentUser(userId);
        return await _playlistsService.RemoveSongAsync(userId, playlistId, songId);
    }

    private void EnsureCurrentUser(Guid userId)
    {
        var subjectClaim = User.FindFirst("sub")?.Value ??
                           User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(subjectClaim, out var currentUserId) || currentUserId != userId)
        {
            throw new AccessDeniedException("User can manage only own playlists");
        }
    }
}
