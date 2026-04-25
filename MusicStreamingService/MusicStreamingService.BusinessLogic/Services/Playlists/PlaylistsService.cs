using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Media;
using MusicStreamingService.BusinessLogic.Services.Playlists.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.DataAccess.Postgres.UnitOfWork.Interfaces;
using MusicStreamingService.MediaLibrary;
using Npgsql;

namespace MusicStreamingService.BusinessLogic.Services.Playlists;

public class PlaylistsService : IPlaylistsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediaStorageService _mediaStorageService;

    public PlaylistsService(IUnitOfWork unitOfWork, IMapper mapper, IMediaStorageService mediaStorageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediaStorageService = mediaStorageService;
    }

    public async Task<PlaylistModel> CreateAsync(Guid userId, CreatePlaylistModel model)
    {
        var uploadedPhotoKey = await _mediaStorageService.UploadAsync(model.Photo, "playlists", Guid.NewGuid());
        try
        {
            var playlist = _mapper.Map<Playlist>(model);
            playlist.UserId = userId;
            playlist.CreatedAt = DateTime.UtcNow;
            playlist.UpdatedAt = playlist.CreatedAt;
            playlist.PhotoObjectKey = uploadedPhotoKey;

            var result = await _unitOfWork.Playlists.SaveAsync(playlist);
            if (result is null)
            {
                await _unitOfWork.RollbackAsync();
                await _mediaStorageService.DeleteAsync(uploadedPhotoKey);
                throw new EntityAlreadyExistsException("Playlist");
            }

            await _unitOfWork.CommitAsync();
            return await MapPlaylistAsync(result);
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

    public async Task<PlaylistModel> UpdateAsync(Guid userId, Guid playlistId, UpdatePlaylistModel model)
    {
        string? uploadedPhotoKey = null;
        try
        {
            var playlist = await GetOwnedPlaylistAsync(userId, playlistId);
            playlist.Name = model.Name ?? playlist.Name;
            var previousPhotoKey = playlist.PhotoObjectKey;
            if (model.Photo is not null)
            {
                uploadedPhotoKey = await _mediaStorageService.UploadAsync(model.Photo, "playlists", playlistId);
                playlist.PhotoObjectKey = uploadedPhotoKey;
            }
            playlist.UpdatedAt = DateTime.UtcNow;

            var updatedPlaylist = _unitOfWork.Playlists.Update(playlist);
            await _unitOfWork.CommitAsync();

            if (!string.IsNullOrWhiteSpace(uploadedPhotoKey) && uploadedPhotoKey != previousPhotoKey)
            {
                await _mediaStorageService.DeleteAsync(previousPhotoKey);
            }

            return await MapPlaylistAsync(updatedPlaylist);
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

    public async Task<PlaylistModel> DeleteAsync(Guid userId, Guid playlistId)
    {
        try
        {
            var playlist = await GetOwnedPlaylistAsync(userId, playlistId);
            _unitOfWork.Playlists.Delete(playlist);
            await _unitOfWork.CommitAsync();
            await _mediaStorageService.DeleteAsync(playlist.PhotoObjectKey);
            return await MapPlaylistAsync(playlist);
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

    public async Task<PlaylistSongModel> AddSongAsync(Guid userId, Guid playlistId, Guid songId)
    {
        try
        {
            var song = await _unitOfWork.Songs.FindByIdAsync(songId);
            if (song is null)
            {
                throw new EntityNotFoundException("Song", songId);
            }
            
            await GetOwnedPlaylistAsync(userId, playlistId);

            var playlistSong = await _unitOfWork.Playlists.AddSongAsync(playlistId, songId);
            if (playlistSong is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityAlreadyExistsException("PlaylistSong");
            }

            await _unitOfWork.CommitAsync();
            return _mapper.Map<PlaylistSongModel>(playlistSong);
        }
        catch (DbUpdateException e)
        {
            await _unitOfWork.RollbackAsync();
            if (e.InnerException is PostgresException { SqlState: "23505" })
            {
                throw new EntityAlreadyExistsException("PlaylistSong");
            }

            throw;
        }
        catch (NpgsqlException)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<PlaylistSongModel> RemoveSongAsync(Guid userId, Guid playlistId, Guid songId)
    {
        try
        {
            await GetOwnedPlaylistAsync(userId, playlistId);

            var playlistSong = await _unitOfWork.Playlists.FindSongAsync(playlistId, songId);
            if (playlistSong is null)
            {
                throw new EntityNotFoundException("PlaylistSong");
            }

            _unitOfWork.Playlists.DeleteSong(playlistSong);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<PlaylistSongModel>(playlistSong);
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

    public async Task<CursorResponse<DateTime?, PlaylistModel>> GetUserPlaylistsAsync(Guid userId, string? namePart,
        PaginationParams<DateTime?> paginationParams)
    {
        var playlists = await _unitOfWork.Playlists.FindAllByUserIdAsync(userId, namePart, paginationParams);
        return new CursorResponse<DateTime?, PlaylistModel>
        {
            Cursor = playlists.Cursor,
            Items = await MapPlaylistsAsync(playlists.Items)
        };
    }

    public async Task<CursorResponse<int?, SongModel>> GetPlaylistSongsAsync(Guid userId, Guid playlistId, string? namePart,
        PaginationParams<int?> paginationParams)
    {
        await GetOwnedPlaylistAsync(userId, playlistId);

        var songs = await _unitOfWork.Playlists.FindSongsAsync(playlistId, namePart, paginationParams);
        return new CursorResponse<int?, SongModel>
        {
            Cursor = songs.Cursor,
            Items = _mapper.Map<List<SongModel>>(songs.Items)
        };
    }

    private async Task<Playlist> GetOwnedPlaylistAsync(Guid userId, Guid playlistId)
    {
        var playlist = await _unitOfWork.Playlists.FindByIdAsync(playlistId);
        if (playlist is null)
        {
            throw new EntityNotFoundException("Playlist", playlistId);
        }

        if (playlist.UserId != userId)
        {
            throw new AccessDeniedException("Playlist does not belong to this user");
        }

        return playlist;
    }

    private async Task<List<PlaylistModel>> MapPlaylistsAsync(IEnumerable<Playlist> playlists)
    {
        var tasks = playlists.Select(MapPlaylistAsync);
        return (await Task.WhenAll(tasks)).ToList();
    }

    private async Task<PlaylistModel> MapPlaylistAsync(Playlist playlist)
    {
        var model = _mapper.Map<PlaylistModel>(playlist);
        model.PhotoUrl = await _mediaStorageService.GetReadUrlAsync(playlist.PhotoObjectKey);
        return model;
    }
}
