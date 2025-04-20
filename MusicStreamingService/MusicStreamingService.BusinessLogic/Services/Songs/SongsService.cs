using System.Data;
using AutoMapper;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.Repositories.Interfaces;
using MusicStreamingService.DataAccess.UnitOfWork.Interfaces;

namespace MusicStreamingService.BusinessLogic.Services.Songs;

public class SongsService : ISongsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SongsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<DateTime?, SongModel>> GetAllSongsAsync(PaginationParams<DateTime?> request)
    {
        var songs = await _unitOfWork.Songs.FindAllAsync(request);
        return new PaginatedResponse<DateTime?, SongModel>
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

    public async Task<PaginatedResponse<DateTime?, SongModel>> GetSongByTitleAsync(string titlePart, 
        PaginationParams<DateTime?> request)
    {
        var songs = await _unitOfWork.Songs.FindByTitlePartAsync(titlePart, request);
        return new PaginatedResponse<DateTime?, SongModel>
        {
            Cursor = songs.Cursor,
            Items = _mapper.Map<List<SongModel>>(songs.Items),
        };
    }

    public async Task<SongModel> CreateSongAsync(CreateSongModel model)
    {
        var song = _mapper.Map<Song>(model);
        
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
            
            await _unitOfWork.CommitAsync();
            return _mapper.Map<SongModel>(song);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
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
            return _mapper.Map<SongModel>(song);
        }
        catch (Exception )
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
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}