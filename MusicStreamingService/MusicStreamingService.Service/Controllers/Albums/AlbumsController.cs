using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MusicStreamingService.BusinessLogic.Services.Albums;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.Service.Controllers.Albums.Models;
using MusicStreamingService.Service.Controllers.Songs.Models;

namespace MusicStreamingService.Service.Controllers.Albums;

[ApiController]
[Route("[controller]")]
public class AlbumsController : ControllerBase
{
    private readonly IAlbumsService _albumsService;
    private readonly IMapper _mapper;
    private readonly ILogger<AlbumsController> _logger;

    public AlbumsController(IAlbumsService albumsService, IMapper mapper, ILogger<AlbumsController> logger)
    {
        _albumsService = albumsService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<AlbumsListResponse>> CreateAlbum([FromBody] CreateAlbumRequest request)
    {
        var createAlbumModel = _mapper.Map<CreateAlbumModel>(request);
        
        var album = await _albumsService.CreateAlbumAsync(createAlbumModel);
        return Ok(new AlbumsListResponse([album]));
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<PaginatedResponse<DateTime?, AlbumModel>>> GetAllAlbums(
        [FromQuery] PaginationParams<DateTime?> request)
    {
        var albums = await _albumsService.GetAllAlbumsAsync(request);
        return Ok(albums);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<AlbumsListResponse>> GetAlbumById(Guid id)
    {
        var album = await _albumsService.GetAlbumByIdAsync(id);
        return Ok(new AlbumsListResponse([album]));
    }
    
    [HttpGet]
    [Route("by_name")]
    public async Task<ActionResult<PaginatedResponse<DateTime?, AlbumModel>>> GetAlbumsByName([FromQuery] string titlePart,
        [FromQuery] PaginationParams<DateTime?> request)
    {
        var albums = await _albumsService.GetAlbumByTitleAsync(titlePart, request);
        return Ok(albums);
    }

    [HttpGet]
    [Route("{id:guid}/songs")]
    public async Task<ActionResult<PaginatedResponse<int?, SongModel>>> GetAlbumsBySongs(Guid id,
        [FromQuery] PaginationParams<int?> request)
    {
        var songs = await _albumsService.GetAllAlbumSongsAsync(id, request);
        return Ok(songs);
    }

    [HttpDelete]
    [Route("delete/{id:guid}")]
    public async Task<ActionResult<AlbumsListResponse>> DeleteAlbum(Guid id)
    {
        var album = await _albumsService.DeleteAlbumAsync(id);
        return Ok(new AlbumsListResponse([album]));
    }

    [HttpPut]
    [Route("update/{id:guid}")]
    public async Task<ActionResult<AlbumsListResponse>> UpdateAlbum(Guid id, [FromBody] UpdateAlbumRequest request)
    {
        var updateAlbumModel = _mapper.Map<UpdateAlbumModel>(request);
        
        var album = await _albumsService.UpdateAlbumAsync(updateAlbumModel, id);
        return Ok(new AlbumsListResponse([album]));
    }
}