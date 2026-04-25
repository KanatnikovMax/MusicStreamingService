using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.DataAccess.Postgres.UnitOfWork.Interfaces;
using MusicStreamingService.MediaLibrary;
using Newtonsoft.Json;
using Npgsql;

namespace MusicStreamingService.BusinessLogic.Services.Artists;

public class ArtistsService : IArtistsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly IMediaStorageService _mediaStorageService;

    public ArtistsService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IDistributedCache cache,
        IMediaStorageService mediaStorageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
        _mediaStorageService = mediaStorageService;
    }

    public async Task<ArtistModel> GetArtistByIdAsync(Guid id)
    {
        var cacheKey = $"artists_{id}";
        var cachedArtist = await _cache.GetStringAsync(cacheKey);
        Artist? artist;
        if (string.IsNullOrEmpty(cachedArtist))
        {
            artist = await _unitOfWork.Artists.FindByIdAsync(id)
                     ?? throw new EntityNotFoundException("Artist", id);
            await _cache.SetStringAsync(
                cacheKey, 
                JsonConvert.SerializeObject(artist, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                }),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                });
            return await MapArtistAsync(artist);
        }

        artist = JsonConvert.DeserializeObject<Artist>(cachedArtist);
        return await MapArtistAsync(artist);
    }

    public async Task<CursorResponse<DateTime?, ArtistModel>> GetArtistByNameAsync(string? namePart, 
        PaginationParams<DateTime?> request)
    {
        var artists = namePart == null 
            ? await _unitOfWork.Artists.FindAllAsync(request) 
            : await _unitOfWork.Artists.FindByNamePartAsync(namePart, request);
        return new CursorResponse<DateTime?, ArtistModel>
        {
            Cursor = artists.Cursor,
            Items = await MapArtistsAsync(artists.Items)
        };
    }

    public async Task<CursorResponse<DateTime?, AlbumModel>> GetAllAlbumsAsync(Guid artistId, 
        PaginationParams<DateTime?> request)
    {
        var albums = await _unitOfWork.Artists.FindAllAlbumsAsync(artistId, request);
        return new CursorResponse<DateTime?, AlbumModel>
        {
            Cursor = albums.Cursor,
            Items = await MapAlbumsAsync(albums.Items)
        };
    }
    
    public async Task<CursorResponse<DateTime?, SongModel>> GetSongsByTitleAsync(Guid artistId, string? titlePart,
        PaginationParams<DateTime?> request)
    {
        var songs = titlePart == null 
            ? await _unitOfWork.Artists.FindAllSongsAsync(artistId, request) 
            : await _unitOfWork.Artists.FindAllSongsByTitleAsync(artistId, titlePart, request);
        return new CursorResponse<DateTime?, SongModel>
        {
            Cursor = songs.Cursor,
            Items = _mapper.Map<List<SongModel>>(songs.Items)
        };
    }

    public async Task<ArtistModel> CreateArtistAsync(CreateArtistModel model)
    {
        var artist = _mapper.Map<Artist>(model);
        var uploadedPhotoKey = await _mediaStorageService.UploadAsync(model.Photo, "artists", Guid.NewGuid());
        artist.PhotoObjectKey = uploadedPhotoKey;
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            artist = await _unitOfWork.Artists.SaveAsync(artist);
            if (artist is null)
            {
                await _unitOfWork.RollbackAsync();
                await _mediaStorageService.DeleteAsync(uploadedPhotoKey);
                throw new EntityAlreadyExistsException("Artist");
            }
            
            await _unitOfWork.CommitAsync();
            return await MapArtistAsync(artist);
        }
        catch (DbUpdateException e)
        {
            await _unitOfWork.RollbackAsync();
            await _mediaStorageService.DeleteAsync(uploadedPhotoKey);
            if (e.InnerException is PostgresException { SqlState: "23505" })
            {
                throw new EntityAlreadyExistsException("Artist");
            }
        
            throw;
        }
        catch (NpgsqlException)
        {
            await _unitOfWork.RollbackAsync();
            await _mediaStorageService.DeleteAsync(uploadedPhotoKey);
            throw;
        }
    }

    public async Task<ArtistModel> DeleteArtistAsync(Guid id)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var cachedKey = $"artists_{id}";
            var cachedArtist = await _cache.GetStringAsync(cachedKey);
            var artist = string.IsNullOrEmpty(cachedArtist) 
                ? await _unitOfWork.Artists.FindByIdAsync(id) 
                : JsonConvert.DeserializeObject<Artist>(cachedArtist);
            
            if (artist is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityNotFoundException("Artist", id);
            }

            var albums = await _unitOfWork.Artists.FindAllAlbumsAsync(id);
            var albumsToDelete = albums.Where(a => a.Artists.Count == 1).ToList();

            foreach (var album in albumsToDelete)
            {
                _unitOfWork.Albums.Delete(album);
            }
            
            await _cache.RemoveAsync(cachedKey);
            
            _unitOfWork.Artists.Delete(artist);
        
            await _unitOfWork.CommitAsync();
            await _mediaStorageService.DeleteAsync(artist.PhotoObjectKey);

            return await MapArtistAsync(artist);
        }
        catch (DbUpdateException)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
        catch (NpgsqlException)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<ArtistModel> UpdateArtistAsync(UpdateArtistModel model, Guid id)
    {
        string? uploadedPhotoKey = null;
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            if (model.Name is not null && await _unitOfWork.Artists.FindByNameAsync(model.Name) is not null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityAlreadyExistsException("Artist");
            }
            
            var cachedKey = $"artists_{id}";
            var cachedArtist = await _cache.GetStringAsync(cachedKey);
            var artist = string.IsNullOrEmpty(cachedArtist) 
                ? await _unitOfWork.Artists.FindByIdAsync(id) 
                : JsonConvert.DeserializeObject<Artist>(cachedArtist);
            
            if (artist is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityNotFoundException("Artist", id);
            }
            
            await _cache.RemoveAsync(cachedKey);
            
            artist.Name = model.Name ?? artist.Name;
            var previousPhotoKey = artist.PhotoObjectKey;
            if (model.Photo is not null)
            {
                uploadedPhotoKey = await _mediaStorageService.UploadAsync(model.Photo, "artists", id);
                artist.PhotoObjectKey = uploadedPhotoKey;
            }

            artist = _unitOfWork.Artists.Update(artist);
            await _unitOfWork.CommitAsync();

            if (!string.IsNullOrWhiteSpace(uploadedPhotoKey) && uploadedPhotoKey != previousPhotoKey)
            {
                await _mediaStorageService.DeleteAsync(previousPhotoKey);
            }

            return await MapArtistAsync(artist);
        }
        catch (DbUpdateException e)
        {
            await _unitOfWork.RollbackAsync();
            await _mediaStorageService.DeleteAsync(uploadedPhotoKey);
            if (e.InnerException is PostgresException { SqlState: "23505" })
            {
                throw new EntityAlreadyExistsException("Artist");
            }
        
            throw;
        }
        catch (NpgsqlException)
        {
            await _unitOfWork.RollbackAsync();
            await _mediaStorageService.DeleteAsync(uploadedPhotoKey);
            throw;
        }
    }

    private async Task<List<ArtistModel>> MapArtistsAsync(IEnumerable<Artist> artists)
    {
        var tasks = artists.Select(MapArtistAsync);
        return (await Task.WhenAll(tasks)).ToList();
    }

    private async Task<ArtistModel> MapArtistAsync(Artist artist)
    {
        var model = _mapper.Map<ArtistModel>(artist);
        model.PhotoUrl = await _mediaStorageService.GetReadUrlAsync(artist.PhotoObjectKey);

        if (artist.Albums is not null && model.Albums.Count != 0)
        {
            var albumPhotoKeys = artist.Albums.ToDictionary(album => album.Id, album => album.PhotoObjectKey);
            var urlTasks = model.Albums.Select(async album =>
            {
                album.PhotoUrl = albumPhotoKeys.TryGetValue(album.Id, out var photoObjectKey)
                    ? await _mediaStorageService.GetReadUrlAsync(photoObjectKey)
                    : null;
            });
            await Task.WhenAll(urlTasks);
        }

        return model;
    }

    private async Task<List<AlbumModel>> MapAlbumsAsync(IEnumerable<Album> albums)
    {
        var tasks = albums.Select(async album =>
        {
            var model = _mapper.Map<AlbumModel>(album);
            model.PhotoUrl = await _mediaStorageService.GetReadUrlAsync(album.PhotoObjectKey);
            return model;
        });

        return (await Task.WhenAll(tasks)).ToList();
    }
}
