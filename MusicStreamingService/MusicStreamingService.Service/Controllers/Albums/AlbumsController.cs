using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MusicStreamingService.BusinessLogic.Services.Albums;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.Service.Controllers.Albums.Models;

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
    public async Task<ActionResult<AlbumsListResponse>> GetAllAlbums()
    {
        var albums = await _albumsService.GetAllAlbumsAsync();
        return Ok(new AlbumsListResponse(albums.ToList()));
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<Album>> GetAlbum(Guid id)
    {
        var album = await _albumsService.GetAlbumByIdAsync(id);
        return Ok(new AlbumsListResponse([album]));
    }
    
    [HttpGet]
    [Route("by_name")]
    public async Task<ActionResult<AlbumsListResponse>> GetArtistByName([FromQuery] string namePart)
    {
        var artists = await _albumsService.GetAlbumByNameAsync(namePart);
        return Ok(new AlbumsListResponse(artists.ToList()));
    }

    [HttpDelete]
    [Route("delete/{id:guid}")]
    public async Task<ActionResult<Album>> DeleteAlbum(Guid id)
    {
        var album = await _albumsService.DeleteAlbumAsync(id);
        return Ok(new AlbumsListResponse([album]));
    }

    [HttpPut]
    [Route("update/{id:guid}")]
    public async Task<ActionResult<Album>> UpdateAlbum(Guid id, [FromBody] UpdateAlbumRequest request)
    {
        var updateAlbumModel = _mapper.Map<UpdateAlbumModel>(request);
        
        var album = await _albumsService.UpdateAlbumAsync(updateAlbumModel, id);
        return Ok(new AlbumsListResponse([album]));
    }
}