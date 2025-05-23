﻿using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.DataAccess.Postgres.UnitOfWork.Interfaces;
using Newtonsoft.Json;
using Npgsql;

namespace MusicStreamingService.BusinessLogic.Services.Artists;

public class ArtistsService : IArtistsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    public ArtistsService(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<CursorResponse<DateTime?, ArtistModel>> GetAllArtistsAsync(PaginationParams<DateTime?> request)
    {
        var artists = await _unitOfWork.Artists.FindAllAsync(request);
        return new CursorResponse<DateTime?, ArtistModel>
        {
            Cursor = artists.Cursor,
            Items = _mapper.Map<List<ArtistModel>>(artists.Items)
        };
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
            
            return _mapper.Map<ArtistModel>(artist);
        }

        artist = JsonConvert.DeserializeObject<Artist>(cachedArtist);
        return _mapper.Map<ArtistModel>(artist);
    }

    public async Task<CursorResponse<DateTime?, ArtistModel>> GetArtistByNameAsync(string namePart, 
        PaginationParams<DateTime?> request)
    {
        var artists = await _unitOfWork.Artists.FindByNamePartAsync(namePart, request);
        return new CursorResponse<DateTime?, ArtistModel>
        {
            Cursor = artists.Cursor,
            Items = _mapper.Map<List<ArtistModel>>(artists.Items)
        };
    }

    public async Task<CursorResponse<DateTime?, AlbumModel>> GetAllAlbumsAsync(Guid artistId, 
        PaginationParams<DateTime?> request)
    {
        var albums = await _unitOfWork.Artists.FindAllAlbumsAsync(artistId, request);
        return new CursorResponse<DateTime?, AlbumModel>
        {
            Cursor = albums.Cursor,
            Items = _mapper.Map<List<AlbumModel>>(albums.Items)
        };
    }
    
    public async Task<CursorResponse<DateTime?, SongModel>> GetAllSongsAsync(Guid artistId, 
        PaginationParams<DateTime?> request)
    {
        var songs = await _unitOfWork.Artists.FindAllSongsAsync(artistId, request);
        return new CursorResponse<DateTime?, SongModel>
        {
            Cursor = songs.Cursor,
            Items = _mapper.Map<List<SongModel>>(songs.Items)
        };
    }
    
    public async Task<CursorResponse<DateTime?, SongModel>> GetSongsByTitleAsync(Guid artistId, string titlePart,
        PaginationParams<DateTime?> request)
    {
        var songs = await _unitOfWork.Artists.FindAllSongsByTitleAsync(artistId, titlePart, request);
        return new CursorResponse<DateTime?, SongModel>
        {
            Cursor = songs.Cursor,
            Items = _mapper.Map<List<SongModel>>(songs.Items)
        };
    }

    public async Task<ArtistModel> CreateArtistAsync(CreateArtistModel model)
    {
        var artist = _mapper.Map<Artist>(model);
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            artist = await _unitOfWork.Artists.SaveAsync(artist);
            if (artist is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityAlreadyExistsException("Artist");
            }
            
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ArtistModel>(artist);
        }
        catch (DbUpdateException e)
        {
            await _unitOfWork.RollbackAsync();
            if (e.InnerException is PostgresException { SqlState: "23505" })
            {
                throw new EntityAlreadyExistsException("Artist");
            }
        
            throw;
        }
        catch (NpgsqlException)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<ArtistModel> DeleteArtistAsync(Guid id)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);
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

            return _mapper.Map<ArtistModel>(artist);
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
            artist = _unitOfWork.Artists.Update(artist);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ArtistModel>(artist);
        }
        catch (DbUpdateException e)
        {
            await _unitOfWork.RollbackAsync();
            if (e.InnerException is PostgresException { SqlState: "23505" })
            {
                throw new EntityAlreadyExistsException("Artist");
            }
        
            throw;
        }
        catch (NpgsqlException)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}