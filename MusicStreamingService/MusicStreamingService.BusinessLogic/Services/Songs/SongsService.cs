using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Cassandra.Repositories.Interfaces;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;
using MusicStreamingService.DataAccess.Postgres.UnitOfWork.Interfaces;
using Npgsql;

namespace MusicStreamingService.BusinessLogic.Services.Songs;

public class SongsService : ISongsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICassandraSongsRepository _cassandraRepository;

    public SongsService(IUnitOfWork unitOfWork, IMapper mapper, ICassandraSongsRepository cassandraRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cassandraRepository = cassandraRepository;
    }

    public async Task<CursorResponse<DateTime?, SongModel>> GetAllSongsAsync(PaginationParams<DateTime?> request)
    {
        var songs = await _unitOfWork.Songs.FindAllAsync(request);
        return new CursorResponse<DateTime?, SongModel>
        {
            Cursor = songs.Cursor,
            Items = _mapper.Map<List<SongModel>>(songs.Items),
        };
    }

    public async Task<SongModel> GetSongByIdAsync(Guid id)
    {
        var song = await _unitOfWork.Songs.FindByIdAsync(id)
            ?? throw new EntityNotFoundException("Song", id);
        return _mapper.Map<SongModel>(song);
    }

    public async Task<CursorResponse<DateTime?, SongModel>> GetSongByTitleAsync(string titlePart, 
        PaginationParams<DateTime?> request)
    {
        var songs = await _unitOfWork.Songs.FindByTitlePartAsync(titlePart, request);
        return new CursorResponse<DateTime?, SongModel>
        {
            Cursor = songs.Cursor,
            Items = _mapper.Map<List<SongModel>>(songs.Items),
        };
    }

    public async Task<SongModel> CreateSongAsync(CreateSongModel model, byte[] audioData)
    {
        var song = _mapper.Map<Song>(model);
        song.CassandraId = Guid.NewGuid();
        
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var album = await _unitOfWork.Albums.FindByIdAsync(model.AlbumId);
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
            
            await _cassandraRepository.SaveAsync(song.CassandraId, audioData);
            
            await _unitOfWork.CommitAsync();
            return _mapper.Map<SongModel>(song);
        }
        catch (DbUpdateException)
        {
            await _unitOfWork.RollbackAsync();
            if (song is not null)
            {
                await _cassandraRepository.DeleteAsync(song.CassandraId);
            }
            throw;
        }
        catch (NpgsqlException)
        {
            await _unitOfWork.RollbackAsync();
            if (song is not null)
            {
                await _cassandraRepository.DeleteAsync(song.CassandraId);
            }
            throw;
        }
    }

    public async Task<SongModel> DeleteSongAsync(Guid id)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var song = await _unitOfWork.Songs.FindByIdAsync(id);
            if (song is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityNotFoundException("Song", id);
            }
            
            _unitOfWork.Songs.Delete(song);
            await _unitOfWork.CommitAsync();
            
            await _cassandraRepository.DeleteAsync(song.CassandraId);
            
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
            var song = await _unitOfWork.Songs.FindByIdAsync(id);
            if (song is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityNotFoundException("Song", id);
            }
            
            song.Title = model?.Title ?? song.Title;
            song.TrackNumber = model?.TrackNumber ?? song.TrackNumber;
            song = _unitOfWork.Songs.Update(song);
            await _unitOfWork.CommitAsync();
            
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

    public async Task<byte[]> GetSongAudioAsync(Guid id)
    {
        var song = await _unitOfWork.Songs.FindByIdAsync(id)
                   ?? throw new EntityNotFoundException("Song", id);
        
        var audio = await _cassandraRepository.FindAsync(song.CassandraId)
            ?? throw new EntityNotFoundException("Audio", song.CassandraId);

        return audio;
    }
}