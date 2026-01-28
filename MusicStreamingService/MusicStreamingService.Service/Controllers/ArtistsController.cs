using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Artists;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.Service.Controllers.Requests.Artists;
using MusicStreamingService.Service.Controllers.Requests.Pagination;
using MusicStreamingService.Service.Controllers.Responses.Artists;
using MusicStreamingService.Service.Controllers.Responses.Pagination;

namespace MusicStreamingService.Service.Controllers;

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
    
    [Authorize(Roles = "admin")]
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<ArtistsListResponse>> CreateArtist([FromForm] CreateArtistRequest request,
        [FromForm] IFormFile? photo)
    {
        var createArtistModel = _mapper.Map<CreateArtistModel>(request);
        if (photo != null)
        {
            using var memoryStream = new MemoryStream();
            await photo.CopyToAsync(memoryStream);
            createArtistModel.Photo = memoryStream.ToArray();
        }
        var artist = await _artistsService.CreateArtistAsync(createArtistModel);
        return Ok(new ArtistsListResponse([artist]));
    }
    
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<PaginatedResponse<DateTime?, ArtistModel>>> GetAllArtists(
        [FromQuery] PaginationRequest<DateTime?> request)
    {
        var paginationParams = _mapper.Map <PaginationParams<DateTime?>>(request);
        var artists = await _artistsService.GetAllArtistsAsync(paginationParams);
        return Ok(_mapper.Map<PaginatedResponse<DateTime?, ArtistModel>>(artists));
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
    public async Task<ActionResult<PaginatedResponse<DateTime?, ArtistModel>>> GetArtistsByName(
        [FromQuery] string namePart, [FromQuery] PaginationRequest<DateTime?> request)
    {
        var paginationParams = _mapper.Map <PaginationParams<DateTime?>>(request); 
        var artists = await _artistsService.GetArtistByNameAsync(namePart, paginationParams);
        return Ok(_mapper.Map<PaginatedResponse<DateTime?, ArtistModel>>(artists));
    }

    [HttpGet]
    [Route("{id:guid}/albums")]
    public async Task<ActionResult<PaginatedResponse<DateTime?, AlbumModel>>> GetAllArtistAlbums(Guid id,
        [FromQuery] PaginationRequest<DateTime?> request)
    {
        var paginationParams = _mapper.Map <PaginationParams<DateTime?>>(request);
        var albums = await _artistsService.GetAllAlbumsAsync(id, paginationParams);
        return Ok(_mapper.Map<PaginatedResponse<DateTime?, AlbumModel>>(albums));
    }
    
    [HttpGet]
    [Route("{id:guid}/songs")]
    public async Task<ActionResult<PaginatedResponse<DateTime?, SongModel>>> GetAllArtistSongs(Guid id,
        [FromQuery] PaginationRequest<DateTime?> request)
    {
        var paginationParams = _mapper.Map <PaginationParams<DateTime?>>(request);
        var songs = await _artistsService.GetAllSongsAsync(id, paginationParams);
        return Ok(_mapper.Map<PaginatedResponse<DateTime?, SongModel>>(songs));
    }
    
    [HttpGet]
    [Route("{id:guid}/songs/by_title")]
    public async Task<ActionResult<PaginatedResponse<DateTime?, SongModel>>> GetArtistSongsByTitle(Guid id, 
        [FromQuery] string titlePart, [FromQuery] PaginationRequest<DateTime?> request)
    {
        var paginationParams = _mapper.Map <PaginationParams<DateTime?>>(request);
        var songs = await _artistsService.GetSongsByTitleAsync(id, titlePart, paginationParams);
        return Ok(_mapper.Map<PaginatedResponse<DateTime?, SongModel>>(songs));
    }
    
    [Authorize(Roles = "admin")]
    [HttpDelete]
    [Route("delete/{id:guid}")]
    public async Task<ActionResult<ArtistsListResponse>> DeleteArtist(Guid id)
    {
        var artist = await _artistsService.DeleteArtistAsync(id);
        return Ok(new ArtistsListResponse([artist]));
    }
    
    [Authorize(Roles = "admin")]
    [HttpPut]
    [Route("update/{id:guid}")]
    public async Task<ActionResult<ArtistsListResponse>> UpdateArtist(Guid id, [FromForm] UpdateArtistRequest request,
        [FromForm] IFormFile? photo)
    {
        var updateArtistModel = _mapper.Map<UpdateArtistModel>(request);
        if (photo != null)
        {
            using var memoryStream = new MemoryStream();
            await photo.CopyToAsync(memoryStream);
            updateArtistModel.Photo = memoryStream.ToArray();
        }
        var artist = await _artistsService.UpdateArtistAsync(updateArtistModel, id);
        return Ok(new ArtistsListResponse([artist]));
    }
}