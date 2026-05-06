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

using MusicStreamingService.Service.Controllers.Responses.Pagination;
using MusicStreamingService.Service.Utils;

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
    public async Task<ActionResult<ArtistModel>> CreateArtist([FromForm] CreateArtistRequest request,
        [FromForm] IFormFile? photo)
    {
        var createArtistModel = _mapper.Map<CreateArtistModel>(request);
        if (photo != null)
        {
            createArtistModel.Photo = await PhotoFilesUtil.CreateFileUploadModelAsync(photo, default);
        }
        var artist = await _artistsService.CreateArtistAsync(createArtistModel);
        return Ok(artist);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<long?, ArtistModel>>> GetArtistsByName(
        [FromQuery] string? namePart, [FromQuery] PaginationRequest<long?> request)
    {
        var paginationParams = _mapper.Map <PaginationParams<long?>>(request); 
        var artists = await _artistsService.GetArtistByNameAsync(namePart, paginationParams);
        return Ok(_mapper.Map<PaginatedResponse<long?, ArtistModel>>(artists));
    }
    
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<ArtistModel>> GetArtistById(Guid id)
    {
        var artist = await _artistsService.GetArtistByIdAsync(id);
        return Ok(artist);
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
    public async Task<ActionResult<PaginatedResponse<long?, SongModel>>> GetArtistSongsByTitle(Guid id,
        [FromQuery] string? titlePart, [FromQuery] PaginationRequest<long?> request)
    {
        var paginationParams = _mapper.Map <PaginationParams<long?>>(request);
        var songs = await _artistsService.GetSongsByTitleAsync(id, titlePart, paginationParams);
        return Ok(_mapper.Map<PaginatedResponse<long?, SongModel>>(songs));
    }

    [Authorize(Roles = "admin")]
    [HttpPut]
    [Route("{id:guid}")]
    public async Task<ActionResult<ArtistModel>> UpdateArtist(Guid id, [FromForm] UpdateArtistRequest request,
        [FromForm] IFormFile? photo)
    {
        var updateArtistModel = _mapper.Map<UpdateArtistModel>(request);
        if (photo != null)
        {
            updateArtistModel.Photo = await PhotoFilesUtil.CreateFileUploadModelAsync(photo, default);
        }
        var artist = await _artistsService.UpdateArtistAsync(updateArtistModel, id);
        return Ok(artist);
    }

    [Authorize(Roles = "admin")]
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<ActionResult<ArtistModel>> DeleteArtist(Guid id)
    {
        var artist = await _artistsService.DeleteArtistAsync(id);
        return Ok(artist);
    }
}
