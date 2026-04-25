using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Media;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.DataAccess.Postgres.UnitOfWork.Interfaces;
using MusicStreamingService.MediaLibrary;
using Newtonsoft.Json;
using Npgsql;

namespace MusicStreamingService.BusinessLogic.Services.Albums;

public class AlbumsService : IAlbumsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly IMediaStorageService _mediaStorageService;

    public AlbumsService(
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

    public async Task<AlbumModel> GetAlbumByIdAsync(Guid id)
    {
        var cacheKey = $"albums_{id}";
        var cachedAlbum = await _cache.GetStringAsync(cacheKey);
        Album? album;
        if (string.IsNullOrEmpty(cachedAlbum))
        {
            album = await _unitOfWork.Albums.FindByIdAsync(id)
                    ?? throw new EntityNotFoundException("Album", id);
            
            await _cache.SetStringAsync(
                cacheKey, 
                JsonConvert.SerializeObject(album, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                }),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                });
            return await MapAlbumAsync(album);
        }
        
        album = JsonConvert.DeserializeObject<Album>(cachedAlbum);
        return await MapAlbumAsync(album);
    }

    public async Task<CursorResponse<DateTime?, AlbumModel>> GetAlbumByTitleAsync(string? titlePart, 
        PaginationParams<DateTime?> request)
    {
        var albums = titlePart == null 
            ? await _unitOfWork.Albums.FindAllAsync(request) 
            : await _unitOfWork.Albums.FindByTitlePartAsync(titlePart, request);
        return new CursorResponse<DateTime?, AlbumModel>
        {
            Cursor = albums.Cursor,
            Items = await MapAlbumsAsync(albums.Items)
        };
    }

    public async Task<CursorResponse<int?, SongModel>> GetAllAlbumSongsAsync(Guid albumId, 
        PaginationParams<int?> request)
    {
        var songs = await _unitOfWork.Albums.FindAllSongsAsync(albumId, request);
        
        return new CursorResponse<int?, SongModel>
        {
            Cursor = songs.Cursor,
            Items = _mapper.Map<List<SongModel>>(songs.Items)
        };
    }
    
    public async Task<AlbumModel> CreateAlbumAsync(CreateAlbumModel model)
    {
        var entity = _mapper.Map<Album>(model);
        string? uploadedPhotoKey = null;
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var album = await _unitOfWork.Albums.FindByTitleAsync(entity.Title);
            if (album is not null)
            {
                var artistsNames = album.Artists
                    .Select(a => a.Name.ToLower())
                    .OrderBy(name => name)
                    .ToList();

                var existingArtistsNames = model.Artists
                    .Select(n => n.ToLower())
                    .OrderBy(name => name)
                    .ToList();

                var isDuplicate = artistsNames
                    .SequenceEqual(existingArtistsNames);

                if (isDuplicate)
                {
                    await _unitOfWork.RollbackAsync();
                    throw new EntityAlreadyExistsException("Album");
                }
            }
        
            var artists = await _unitOfWork.Artists.GetOrCreateArtistsAsync(model.Artists);
            entity.Artists = artists;
            uploadedPhotoKey = await _mediaStorageService.UploadAsync(model.Photo, "albums", Guid.NewGuid());
            entity.PhotoObjectKey = uploadedPhotoKey;
            entity = await _unitOfWork.Albums.SaveAsync(entity);
            if (entity is null)
            {
                await _unitOfWork.RollbackAsync();
                await _mediaStorageService.DeleteAsync(uploadedPhotoKey);
                throw new EntityAlreadyExistsException("Album");
            }
            await _unitOfWork.CommitAsync();
        
            return await MapAlbumAsync(entity);
        }
        catch (EntityAlreadyExistsException)
        {
            await _mediaStorageService.DeleteAsync(uploadedPhotoKey);
            throw;
        }
        catch (DbUpdateException)
        {
            await _unitOfWork.RollbackAsync();
            await _mediaStorageService.DeleteAsync(uploadedPhotoKey);
            throw;
        }
        catch (NpgsqlException)
        {
            await _unitOfWork.RollbackAsync();
            await _mediaStorageService.DeleteAsync(uploadedPhotoKey);
            throw;
        }
    }

    public async Task<AlbumModel> DeleteAlbumAsync(Guid id)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var cacheKey = $"albums_{id}";
            var cachedAlbum = await _cache.GetStringAsync(cacheKey);
            var album = string.IsNullOrEmpty(cachedAlbum) 
                ? await _unitOfWork.Albums.FindByIdAsync(id)
                : JsonConvert.DeserializeObject<Album>(cachedAlbum);
            
            if (album is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityNotFoundException("Album", id);
            }
            
            await _cache.RemoveAsync(cacheKey);
            
            _unitOfWork.Albums.Delete(album);
            await _unitOfWork.CommitAsync();
            await _mediaStorageService.DeleteAsync(album.PhotoObjectKey);
            return await MapAlbumAsync(album);
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

    public async Task<AlbumModel> UpdateAlbumAsync(UpdateAlbumModel model, Guid id)
    {
        string? uploadedPhotoKey = null;
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var cacheKey = $"albums_{id}";
            var cachedAlbum = await _cache.GetStringAsync(cacheKey);
            var album = string.IsNullOrEmpty(cachedAlbum) 
                ? await _unitOfWork.Albums.FindByIdAsync(id)
                : JsonConvert.DeserializeObject<Album>(cachedAlbum);
            
            if (album is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityNotFoundException("Album", id);
            }

            if (!string.IsNullOrEmpty(model.Title))
            {
                album.Title = model.Title;
            }
            var previousPhotoKey = album.PhotoObjectKey;
            if (model.Photo is not null)
            {
                uploadedPhotoKey = await _mediaStorageService.UploadAsync(model.Photo, "albums", id);
                album.PhotoObjectKey = uploadedPhotoKey;
            }
            if (model.ReleaseDate.HasValue)
            {
                album.ReleaseDate = model.ReleaseDate.Value;
            }
            
            var artists = await _unitOfWork.Artists.GetOrCreateArtistsAsync(model.Artists);
            
            album.Artists.Clear();
            foreach (var artist in artists)
            {
                album.Artists.Add(artist);
            }
            
            await _cache.RemoveAsync(cacheKey);
            
            _unitOfWork.Albums.Update(album);
            await _unitOfWork.CommitAsync();

            if (!string.IsNullOrWhiteSpace(uploadedPhotoKey) && uploadedPhotoKey != previousPhotoKey)
            {
                await _mediaStorageService.DeleteAsync(previousPhotoKey);
            }

            return await MapAlbumAsync(album);
        }
        catch (DbUpdateException)
        {
            await _unitOfWork.RollbackAsync();
            await _mediaStorageService.DeleteAsync(uploadedPhotoKey);
            throw;
        }
        catch (NpgsqlException)
        {
            await _unitOfWork.RollbackAsync();
            await _mediaStorageService.DeleteAsync(uploadedPhotoKey);
            throw;
        }
    }

    private async Task<List<AlbumModel>> MapAlbumsAsync(IEnumerable<Album> albums)
    {
        var tasks = albums.Select(MapAlbumAsync);
        return (await Task.WhenAll(tasks)).ToList();
    }

    private async Task<AlbumModel> MapAlbumAsync(Album album)
    {
        var model = _mapper.Map<AlbumModel>(album);
        model.PhotoUrl = await _mediaStorageService.GetReadUrlAsync(album.PhotoObjectKey);
        return model;
    }
}
