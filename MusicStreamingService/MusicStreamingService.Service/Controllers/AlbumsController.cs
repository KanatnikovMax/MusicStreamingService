using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamingService.BusinessLogic.Services.Albums;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.Service.Controllers.Requests.Albums;
using MusicStreamingService.Service.Controllers.Requests.Pagination;
using MusicStreamingService.Service.Controllers.Responses.Albums;
using MusicStreamingService.Service.Controllers.Responses.Pagination;
using MusicStreamingService.Service.Utils;

namespace MusicStreamingService.Service.Controllers;

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

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<ActionResult<AlbumsListResponse>> CreateAlbum([FromForm] CreateAlbumRequest request,
        [FromForm] IFormFile? photo)
    {
        var createAlbumModel = _mapper.Map<CreateAlbumModel>(request);
        if (photo != null)
        {
            createAlbumModel.Photo = await PhotoFilesUtil.CreateFileUploadModelAsync(photo, default);
        }
        var album = await _albumsService.CreateAlbumAsync(createAlbumModel);
        return Ok(new AlbumsListResponse([album]));
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<AlbumsListResponse>> GetAlbumById(Guid id)
    {
        var album = await _albumsService.GetAlbumByIdAsync(id);
        return Ok(new AlbumsListResponse([album]));
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<DateTime?, AlbumModel>>> GetAlbumsByName(
        [FromQuery] string? titlePart,
        [FromQuery] PaginationRequest<DateTime?> request)
    {
        var paginationParams = _mapper.Map <PaginationParams<DateTime?>>(request);
        var albums = await _albumsService.GetAlbumByTitleAsync(titlePart, paginationParams);
        return Ok(_mapper.Map<PaginatedResponse<DateTime?, AlbumModel>>(albums));
    }

    [HttpGet]
    [Route("{id:guid}/songs")]
    public async Task<ActionResult<PaginatedResponse<int?, SongModel>>> GetAlbumSongs(Guid id,
        [FromQuery] PaginationRequest<int?> request)
    {
        var paginationParams = _mapper.Map <PaginationParams<int?>>(request);
        var songs = await _albumsService.GetAllAlbumSongsAsync(id, paginationParams);
        return Ok(_mapper.Map<PaginatedResponse<int?, SongModel>>(songs));
    }

    [Authorize(Roles = "admin")]
    [HttpPut]
    [Route("{id:guid}")]
    public async Task<ActionResult<AlbumsListResponse>> UpdateAlbum(Guid id, [FromForm] UpdateAlbumRequest request,
        [FromForm] IFormFile? photo)
    {
        var updateAlbumModel = _mapper.Map<UpdateAlbumModel>(request);
        if (photo != null)
        {
            updateAlbumModel.Photo = await PhotoFilesUtil.CreateFileUploadModelAsync(photo, default);
        }
        var album = await _albumsService.UpdateAlbumAsync(updateAlbumModel, id);
        return Ok(new AlbumsListResponse([album]));
    }

    [Authorize(Roles = "admin")]
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<ActionResult<AlbumsListResponse>> DeleteAlbum(Guid id)
    {
        var album = await _albumsService.DeleteAlbumAsync(id);
        return Ok(new AlbumsListResponse([album]));
    }
}
