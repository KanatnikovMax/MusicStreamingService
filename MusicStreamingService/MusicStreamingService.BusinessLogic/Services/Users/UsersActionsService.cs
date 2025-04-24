using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.BusinessLogic.Services.Users.Models;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.UnitOfWork.Interfaces;
using Npgsql;

namespace MusicStreamingService.BusinessLogic.Services.Users;

public class UsersActionsService : IUsersActionsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UsersActionsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserSongModel> AddSongToAccountAsync(Guid userId, Guid songId)
    {
        try
        {
            var song = await _unitOfWork.Users.AddSongAsync(userId, songId);
            if (song is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityAlreadyExistsException("UserSong");
            }
            await _unitOfWork.CommitAsync();
            return _mapper.Map<UserSongModel>(song);
        }
        catch (DbUpdateException e)
        {
            await _unitOfWork.RollbackAsync();
            if (e.InnerException is PostgresException { SqlState: "23505" })
            {
                throw new EntityAlreadyExistsException("UserSong");
            }
        
            throw;
        }
        catch (NpgsqlException)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<UserSongModel> DeleteSongFromAccountAsync(Guid userId, Guid songId)
    {
        try
        {
            var userSong = await _unitOfWork.Users.FindSongByIdAsync(userId, songId);
            if (userSong is null)
            {
                throw new EntityNotFoundException("UserSong");
            }
        
            _unitOfWork.Users.DeleteSong(userSong);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<UserSongModel>(userSong);
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

    public async Task<UserAlbumModel> AddAlbumToAccountAsync(Guid userId, Guid albumId)
    {
        try
        {
            var album = await _unitOfWork.Users.AddAlbumAsync(userId, albumId);
            if (album is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityAlreadyExistsException("UserAlbum");
            }
            await _unitOfWork.CommitAsync();
            return _mapper.Map<UserAlbumModel>(album);
        }
        catch (DbUpdateException e)
        {
            await _unitOfWork.RollbackAsync();
            if (e.InnerException is PostgresException { SqlState: "23505" })
            {
                throw new EntityAlreadyExistsException("UserAlbum");
            }
        
            throw;
        }
        catch (NpgsqlException)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<UserAlbumModel> DeleteAlbumFromAccountAsync(Guid userId, Guid albumId)
    {
        try
        {
            var userAlbum = await _unitOfWork.Users.FindAlbumByIdAsync(userId, albumId);
            if (userAlbum is null)
            {
                throw new EntityNotFoundException("UserSong");
            }
        
            _unitOfWork.Users.DeleteAlbum(userAlbum);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<UserAlbumModel>(userAlbum);
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

    public async Task<CursorResponse<DateTime?, SongModel>> GetAllUserSongsAsync(Guid userId, 
        PaginationParams<DateTime?> paginationParams)
    {
        var songs = await _unitOfWork.Users.FindAllSongsAsync(userId, paginationParams);
        return new CursorResponse<DateTime?, SongModel>
        {
            Cursor = songs.Cursor,
            Items = _mapper.Map<List<SongModel>>(songs.Items)
        };
    }
    
    public async Task<CursorResponse<DateTime?, SongModel>> GetUserSongsByNameAsync(Guid userId, string namePart, 
        PaginationParams<DateTime?> paginationParams)
    {
        var songs = await _unitOfWork.Users.FindAllSongsByNameAsync(userId, namePart, paginationParams);
        return new CursorResponse<DateTime?, SongModel>
        {
            Cursor = songs.Cursor,
            Items = _mapper.Map<List<SongModel>>(songs.Items)
        };
    }
    
    public async Task<CursorResponse<DateTime?, AlbumModel>> GetAllUserAlbumsAsync(Guid userId, 
        PaginationParams<DateTime?> paginationParams)
    {
        var albums = await _unitOfWork.Users.FindAllAlbumsAsync(userId, paginationParams);
        return new CursorResponse<DateTime?, AlbumModel>
        {
            Cursor = albums.Cursor,
            Items = _mapper.Map<List<AlbumModel>>(albums.Items)
        };
    }

    public async Task<CursorResponse<DateTime?, AlbumModel>> GetUserAlbumsByTitleAsync(Guid userId, string titlePart,
        PaginationParams<DateTime?> paginationParams)
    {
        var albums = await _unitOfWork.Users.FindAllAlbumsByTitleAsync(userId, titlePart, paginationParams);
        return new CursorResponse<DateTime?, AlbumModel>
        {
            Cursor = albums.Cursor,
            Items = _mapper.Map<List<AlbumModel>>(albums.Items)
        };
    }
}