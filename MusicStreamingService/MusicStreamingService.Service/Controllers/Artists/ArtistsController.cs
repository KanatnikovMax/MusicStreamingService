using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Artists;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.Service.Controllers.Artists.Models;
using MusicStreamingService.Service.Controllers.Songs.Models;

namespace MusicStreamingService.Service.Controllers.Artists;

[ApiController]
[Route("[controller]")]
public class ArtistsController : ControllerBase
{
    private readonly IArtistsService _artistsService;
    private readonly IMapper _mapper;
    private readonly ILogger<ArtistsController> _logger;

    public ArtistsController(IArtistsService artistsService, IMapper mapper, ILogger<ArtistsController> logger)
    {
        _artistsService = artistsService;
        _mapper = mapper;
        _logger = logger;
    }
    
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<ArtistsListResponse>> CreateArtist([FromBody] CreateArtistRequest request)
    {
        var createArtistModel = _mapper.Map<CreateArtistModel>(request);
        
        var artist = await _artistsService.CreateArtistAsync(createArtistModel);
        return Ok(new ArtistsListResponse([artist]));
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<PaginatedResponse<DateTime?, ArtistModel>>> GetAllArtists(
        [FromQuery] PaginationParams<DateTime?> request)
    {
        var artists = await _artistsService.GetAllArtistsAsync(request);
        return Ok(artists);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<ArtistsListResponse>> GetArtistById(Guid id)
    {
        var artist = await _artistsService.GetArtistByIdAsync(id);
        return Ok(new ArtistsListResponse([artist]));
    }

    [HttpGet]
    [Route("by_name")]
    public async Task<ActionResult<PaginatedResponse<DateTime?, ArtistModel>>> GetArtistsByName([FromQuery] string namePart,
        [FromQuery] PaginationParams<DateTime?> request)
    {
        var artists = await _artistsService.GetArtistByNameAsync(namePart, request);
        return Ok(artists);
    }

    [HttpGet]
    [Route("{id:guid}/albums")]
    public async Task<ActionResult<PaginatedResponse<DateTime?, AlbumModel>>> GetAllArtistAlbums(Guid id,
        [FromQuery] PaginationParams<DateTime?> request)
    {
        var albums = await _artistsService.GetAllAlbumsAsync(id, request);
        return Ok(albums);
    }
    
    [HttpGet]
    [Route("{id:guid}/songs")]
    public async Task<ActionResult<PaginatedResponse<DateTime?, SongModel>>> GetAllArtistSongs(Guid id,
        [FromQuery] PaginationParams<DateTime?> request)
    {
        var songs = await _artistsService.GetAllSongsAsync(id, request);
        return Ok(songs);
    }
    
    [HttpGet]
    [Route("{id:guid}/songs/by_title")]
    public async Task<ActionResult<PaginatedResponse<DateTime?, SongModel>>> GetArtistSongsByTitle(Guid id, 
        [FromQuery] string titlePart, [FromQuery] PaginationParams<DateTime?> request)
    {
        var songs = await _artistsService.GetSongsByTitleAsync(id, titlePart, request);
        return Ok(songs);
    }
    
    [HttpDelete]
    [Route("delete/{id:guid}")]
    public async Task<ActionResult<ArtistsListResponse>> DeleteArtist(Guid id)
    {
        var artist = await _artistsService.DeleteArtistAsync(id);
        return Ok(new ArtistsListResponse([artist]));
    }

    [HttpPut]
    [Route("update/{id:guid}")]
    public async Task<ActionResult<ArtistsListResponse>> UpdateArtist(Guid id, [FromBody] UpdateArtistRequest request)
    {
        var updateArtistModel = _mapper.Map<UpdateArtistModel>(request);
        
        var artist = await _artistsService.UpdateArtistAsync(updateArtistModel, id);
        return Ok(new ArtistsListResponse([artist]));
    }
}