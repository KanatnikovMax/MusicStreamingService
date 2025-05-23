﻿using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.DataAccess.Postgres.UnitOfWork.Interfaces;
using Newtonsoft.Json;
using Npgsql;

namespace MusicStreamingService.BusinessLogic.Services.Albums;

public class AlbumsService : IAlbumsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    public AlbumsService(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<CursorResponse<DateTime?, AlbumModel>> GetAllAlbumsAsync(PaginationParams<DateTime?> request)
    {
        var albums = await _unitOfWork.Albums.FindAllAsync(request);
        return new CursorResponse<DateTime?, AlbumModel>
        {
            Cursor = albums.Cursor,
            Items = _mapper.Map<List<AlbumModel>>(albums.Items)
        };
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
            
            return _mapper.Map<AlbumModel>(album);
        }
        
        album = JsonConvert.DeserializeObject<Album>(cachedAlbum);
        return _mapper.Map<AlbumModel>(album);
    }

    public async Task<CursorResponse<DateTime?, AlbumModel>> GetAlbumByTitleAsync(string titlePart, 
        PaginationParams<DateTime?> request)
    {
        var albums = await _unitOfWork.Albums.FindByTitlePartAsync(titlePart, request);
        return new CursorResponse<DateTime?, AlbumModel>
        {
            Cursor = albums.Cursor,
            Items = _mapper.Map<List<AlbumModel>>(albums.Items)
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
            entity = await _unitOfWork.Albums.SaveAsync(entity);
            if (entity is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityAlreadyExistsException("Album");
            }
            await _unitOfWork.CommitAsync();
        
            return _mapper.Map<AlbumModel>(entity);
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
            return _mapper.Map<AlbumModel>(album);
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
            if (model.ReleaseDate.HasValue)
            {
                album.ReleaseDate = model.ReleaseDate.Value;
            }
            if (model.Artists != null)
            {
                var artists = await _unitOfWork.Artists.GetOrCreateArtistsAsync(model.Artists);
                
                album.Artists.Clear();
                foreach (var artist in artists)
                {
                    album.Artists.Add(artist);
                }
            }
            
            await _cache.RemoveAsync(cacheKey);
            
            _unitOfWork.Albums.Update(album);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<AlbumModel>(album);
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
}