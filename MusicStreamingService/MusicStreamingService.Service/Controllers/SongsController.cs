using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamingService.BusinessLogic.Services.Songs;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.Service.Controllers.Requests.Pagination;
using MusicStreamingService.Service.Controllers.Requests.Songs;
using MusicStreamingService.Service.Controllers.Responses.Pagination;
using MusicStreamingService.Service.Controllers.Responses.Songs;

namespace MusicStreamingService.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class SongsController : ControllerBase
{
    private readonly ISongsService _songsService;
    private readonly IMapper _mapper;
    private readonly ILogger<SongsController> _logger;

    public SongsController(ISongsService songsService, IMapper mapper, ILogger<SongsController> logger)
    {
        _songsService = songsService;
        _mapper = mapper;
        _logger = logger;
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<SongsListResponse>> CreateSong([FromBody] CreateSongRequest model)
    {
        var createSongModel = _mapper.Map<CreateSongModel>(model);
        
        var song = await _songsService.CreateSongAsync(createSongModel);
        return Ok(new SongsListResponse([song]));
    }
    
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<PaginatedResponse<DateTime?, SongModel>>> GetAllSongs(
        [FromQuery] PaginationRequest<DateTime?> request)
    {
        var paginationParams = _mapper.Map <PaginationParams<DateTime?>>(request);
        var songs = await _songsService.GetAllSongsAsync(paginationParams);
        return Ok(_mapper.Map<PaginatedResponse<DateTime?, SongModel>>(songs));
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<SongsListResponse>> GetSongById(Guid id)
    {
        var song = await _songsService.GetSongByIdAsync(id);
        return Ok(new SongsListResponse([song]));
    }
    
    [HttpGet]
    [Route("by_title")]
    public async Task<ActionResult<PaginatedResponse<DateTime?, SongModel>>> GetSongsByName([FromQuery] string namePart, 
        [FromQuery] PaginationRequest<DateTime?> request)
    {
        var paginationParams = _mapper.Map <PaginationParams<DateTime?>>(request);
        var songs = await _songsService.GetSongByTitleAsync(namePart, paginationParams);
        return Ok(_mapper.Map<PaginatedResponse<DateTime?, SongModel>>(songs));
    }
    
    [Authorize(Roles = "admin")]
    [HttpDelete]
    [Route("delete/{id:guid}")]
    public async Task<ActionResult<SongsListResponse>> DeleteSong(Guid id)
    {
        var song = await _songsService.DeleteSongAsync(id);
        return Ok(new SongsListResponse([song]));
    }
    
    [Authorize(Roles = "admin")]
    [HttpPut]
    [Route("update/{id:guid}")]
    public async Task<ActionResult<SongsListResponse>> UpdateSong(Guid id, [FromBody] UpdateSongRequest request)
    {
        var updateSongModel = _mapper.Map<UpdateSongModel>(request);
        
        var song = await _songsService.UpdateSongAsync(updateSongModel, id);
        return Ok(new SongsListResponse([song]));
    }
}