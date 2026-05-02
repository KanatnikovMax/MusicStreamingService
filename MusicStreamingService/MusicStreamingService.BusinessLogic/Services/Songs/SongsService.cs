using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Media.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.DataAccess.Postgres.UnitOfWork.Interfaces;
using MusicStreamingService.MediaLibrary;
using Newtonsoft.Json;
using Npgsql;

namespace MusicStreamingService.BusinessLogic.Services.Songs;

public class SongsService : ISongsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly IMediaStorageService  _mediaStorageService;

    public SongsService(
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

    public async Task<SongModel> GetSongByIdAsync(Guid id)
    {
        var cacheKey = $"songs_{id}";
        var cashedSong = await _cache.GetStringAsync(cacheKey);
        Song? song;
        if (string.IsNullOrEmpty(cashedSong))
        {
            song = await _unitOfWork.Songs.FindByIdAsync(id)
                   ?? throw new EntityNotFoundException("Song", id);
            
            await _cache.SetStringAsync(
                cacheKey, 
                JsonConvert.SerializeObject(song, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                }),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                });
            
            return _mapper.Map<SongModel>(song);
        }
        
        song = JsonConvert.DeserializeObject<Song>(cashedSong);
        return _mapper.Map<SongModel>(song);
    }

    public async Task<CursorResponse<DateTime?, SongModel>> GetSongByTitleAsync(string? titlePart, 
        PaginationParams<DateTime?> request)
    {
        var songs = titlePart == null 
            ? await _unitOfWork.Songs.FindAllAsync(request) 
            : await _unitOfWork.Songs.FindByTitlePartAsync(titlePart, request);
        return new CursorResponse<DateTime?, SongModel>
        {
            Cursor = songs.Cursor,
            Items = _mapper.Map<List<SongModel>>(songs.Items),
        };
    }

    public async Task<SongModel> CreateSongAsync(CreateSongModel model, byte[] audioData)
    {
        var song = _mapper.Map<Song>(model);
        song.Id = Guid.NewGuid();
        
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var cacheKey = $"albums_{model.AlbumId}";
            var cachedAlbum = await _cache.GetStringAsync(cacheKey);
            var album = string.IsNullOrEmpty(cachedAlbum) 
                ? await _unitOfWork.Albums.FindByIdAsync(model.AlbumId)
                : JsonConvert.DeserializeObject<Album>(cachedAlbum);
            
            if (album is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityNotFoundException("Album", model.AlbumId);
            }
        
            if (!album.Artists.Any(a => model.Artists.Contains(a.Name, StringComparer.OrdinalIgnoreCase)))
            {
                await _unitOfWork.RollbackAsync();
                throw new WrongArtistNameConsistencyException();
            }
            song.Album = album;
            song.Artists = await _unitOfWork.Artists.GetOrCreateArtistsAsync(model.Artists);

            song = await _unitOfWork.Songs.SaveAsync(song);
            
            if (song is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityAlreadyExistsException("song");
            }
            
            using var audioStream = new MemoryStream(audioData);
            var audioObjectKey = await _mediaStorageService.UploadAsync(
                new FileUploadModel
                {
                    Content = audioStream,
                    FileName = $"{song.Id}.mp3",
                    ContentType = "audio/mpeg"
                },
                "songs",
                song.Id);
            
            if (string.IsNullOrEmpty(audioObjectKey))
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception("Failed to upload audio file to MinIO");
            }
            
            song.AudioObjectKey = audioObjectKey;
            await _unitOfWork.CommitAsync();
            
            return _mapper.Map<SongModel>(song);
        }
        catch (DbUpdateException)
        {
            await _unitOfWork.RollbackAsync();
            if (song?.AudioObjectKey is not null)
            {
                await _mediaStorageService.DeleteAsync(song.AudioObjectKey);
            }
            throw;
        }
        catch (NpgsqlException)
        {
            await _unitOfWork.RollbackAsync();
            if (song?.AudioObjectKey is not null)
            {
                await _mediaStorageService.DeleteAsync(song.AudioObjectKey);
            }
            throw;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            if (song?.AudioObjectKey is not null)
            {
                await _mediaStorageService.DeleteAsync(song.AudioObjectKey);
            }
            throw;
        }
    }

    public async Task<SongModel> DeleteSongAsync(Guid id)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var cacheKey = $"songs_{id}";
            var cashedSong = await _cache.GetStringAsync(cacheKey);
            var song = string.IsNullOrWhiteSpace(cashedSong) 
                ? await _unitOfWork.Songs.FindByIdAsync(id) 
                : JsonConvert.DeserializeObject<Song>(cashedSong);
            if (song is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityNotFoundException("Song", id);
            }
            
            var audioObjectKey = song.AudioObjectKey;
            
            _unitOfWork.Songs.Delete(song);
            await _unitOfWork.CommitAsync();
            
            await _cache.RemoveAsync(cacheKey);
            
            if (!string.IsNullOrEmpty(audioObjectKey))
            {
                await _mediaStorageService.DeleteAsync(audioObjectKey);
            }
            
            return _mapper.Map<SongModel>(song);
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

    public async Task<SongModel> UpdateSongAsync(UpdateSongModel model, Guid id)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var cacheKey = $"songs_{id}";
            var cashedSong = await _cache.GetStringAsync(cacheKey);
            var song = string.IsNullOrWhiteSpace(cashedSong) 
                ? await _unitOfWork.Songs.FindByIdAsync(id) 
                : JsonConvert.DeserializeObject<Song>(cashedSong);
            
            if (song is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityNotFoundException("Song", id);
            }
            
            song.Title = model?.Title ?? song.Title;
            song.TrackNumber = model?.TrackNumber ?? song.TrackNumber;
            
            song = _unitOfWork.Songs.Update(song);
            await _unitOfWork.CommitAsync();
            
            await _cache.RemoveAsync(cacheKey);
            
            return _mapper.Map<SongModel>(song);
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

    public async Task<string?> GetSongAudioUrlAsync(Guid id)
    {
        var song = await GetSongByIdAsync(id);
        
        if (string.IsNullOrEmpty(song.AudioObjectKey))
        {
            throw new EntityNotFoundException("Audio", id);
        }

        var url = await _mediaStorageService.GetReadUrlAsync(song.AudioObjectKey);
        return url;
    }
}