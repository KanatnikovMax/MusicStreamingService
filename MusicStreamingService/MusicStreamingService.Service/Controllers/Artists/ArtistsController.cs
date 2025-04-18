using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MusicStreamingService.BusinessLogic.Services.Artists;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.Service.Controllers.Albums.Models;
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
    public async Task<ActionResult<ArtistsListResponse>> GetAllArtists()
    {
        var artists = await _artistsService.GetAllArtistsAsync();
        return Ok(new ArtistsListResponse(artists.ToList()));
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
    public async Task<ActionResult<ArtistsListResponse>> GetArtistsByName([FromQuery] string namePart)
    {
        var artists = await _artistsService.GetArtistByNameAsync(namePart);
        return Ok(new ArtistsListResponse(artists.ToList()));
    }

    [HttpGet]
    [Route("{id:guid}/albums")]
    public async Task<ActionResult<AlbumsListResponse>> GetAllArtistAlbums(Guid id)
    {
        var albums = await _artistsService.GetAllAlbumsAsync(id);
        return Ok(new AlbumsListResponse(albums.ToList()));
    }
    
    [HttpGet]
    [Route("{id:guid}/songs")]
    public async Task<ActionResult<SongsListResponse>> GetAllArtistSongs(Guid id)
    {
        var songs = await _artistsService.GetAllSongsAsync(id);
        return Ok(new SongsListResponse(songs.ToList()));
    }
    
    [HttpGet]
    [Route("{id:guid}/songs/by_title")]
    public async Task<ActionResult<SongsListResponse>> GetArtistSongsByTitle(Guid id, [FromQuery] string titlePart)
    {
        var songs = await _artistsService.GetSongsByTitleAsync(id, titlePart);
        return Ok(new SongsListResponse(songs.ToList()));
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