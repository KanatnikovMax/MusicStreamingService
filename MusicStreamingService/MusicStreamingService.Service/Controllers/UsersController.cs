using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.BusinessLogic.Services.Users;
using MusicStreamingService.BusinessLogic.Services.Users.Models;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.Service.Controllers.Requests.Pagination;
using MusicStreamingService.Service.Controllers.Responses.Pagination;

namespace MusicStreamingService.Service.Controllers;

[ApiController]
[Authorize]
[Route("[controller]/{userId:guid}")]
public class UsersController : ControllerBase
{
    private readonly IUsersActionsService _usersActionsService;
    private readonly IMapper _mapper;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUsersActionsService usersActionsService, IMapper mapper, ILogger<UsersController> logger)
    {
        _usersActionsService = usersActionsService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    [Route("add_song")]
    public async Task<ActionResult<UserSongModel>> AddSongToAccount(Guid userId, [FromQuery] Guid songId)
    {
        return await _usersActionsService.AddSongToAccountAsync(userId, songId);
    }
    
    [HttpPost]
    [Route("add_album")]
    public async Task<ActionResult<UserAlbumModel>> AddAlbumToAccount(Guid userId, [FromQuery] Guid albumId)
    {
        return await _usersActionsService.AddAlbumToAccountAsync(userId, albumId);
    }

    [HttpDelete]
    [Route("delete_song")]
    public async Task<ActionResult<UserSongModel>> DeleteSongFromAccount(Guid userId, Guid songId)
    {
        return await _usersActionsService.DeleteSongFromAccountAsync(userId, songId);
    }

    [HttpDelete]
    [Route("delete_album")]
    public async Task<ActionResult<UserAlbumModel>> DeleteAlbumFromAccount(Guid userId, Guid albumId)
    {
        return await _usersActionsService.DeleteAlbumFromAccountAsync(userId, albumId);
    }

    [HttpGet]
    [Route("albums")]
    public async Task<PaginatedResponse<DateTime?, AlbumModel>> GetAllAlbumsFromAccount(Guid userId,
        [FromQuery] PaginationRequest<DateTime?> request) 
    {
        var paginationParams = _mapper.Map<PaginationParams<DateTime?>>(request);
        var albums = await _usersActionsService.GetAllUserAlbumsAsync(userId, paginationParams);
        return _mapper.Map<PaginatedResponse<DateTime?, AlbumModel>>(albums);
    }
    
    [HttpGet]
    [Route("songs")]
    public async Task<PaginatedResponse<DateTime?, SongModel>> GetAllSongsFromAccount(Guid userId,
        [FromQuery] PaginationRequest<DateTime?> request) 
    {
        var paginationParams = _mapper.Map<PaginationParams<DateTime?>>(request);
        var songs = await _usersActionsService.GetAllUserSongsAsync(userId, paginationParams);
        return _mapper.Map<PaginatedResponse<DateTime?, SongModel>>(songs);
    }
    
    [HttpGet]
    [Route("albums/by_title")]
    public async Task<PaginatedResponse<DateTime?, AlbumModel>> GetAllAlbumsFromAccountByTitle(Guid userId, 
        [FromQuery] string titlePart, [FromQuery] PaginationRequest<DateTime?> request) 
    {
        var paginationParams = _mapper.Map<PaginationParams<DateTime?>>(request);
        var albums = await _usersActionsService.GetUserAlbumsByTitleAsync(userId, titlePart, paginationParams);
        return _mapper.Map<PaginatedResponse<DateTime?, AlbumModel>>(albums);
    }
    
    [HttpGet]
    [Route("songs/by_name")]
    public async Task<PaginatedResponse<DateTime?, SongModel>> GetAllSongsFromAccountByName(Guid userId,
        [FromQuery] string namePart, [FromQuery] PaginationRequest<DateTime?> request) 
    {
        var paginationParams = _mapper.Map<PaginationParams<DateTime?>>(request);
        var songs = await _usersActionsService.GetUserSongsByNameAsync(userId, namePart, paginationParams);
        return _mapper.Map<PaginatedResponse<DateTime?, SongModel>>(songs);
    }
}