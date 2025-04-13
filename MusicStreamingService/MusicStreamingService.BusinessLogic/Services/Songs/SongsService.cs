using AutoMapper;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.Repositories.Interfaces;

namespace MusicStreamingService.BusinessLogic.Services.Songs;

public class SongsService : ISongsService
{
    private readonly ISongsRepository _songsRepository;
    private readonly IArtistsRepository _artistsRepository;
    private readonly IAlbumsRepository _albumsRepository;
    private readonly IMapper _mapper;

    public SongsService(ISongsRepository songsRepository, IMapper mapper)
    {
        _songsRepository = songsRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SongModel>> GetAllSongsAsync()
    {
        var songs = await _songsRepository.FindAllAsync();
        return _mapper.Map<IEnumerable<SongModel>>(songs);
    }

    public async Task<SongModel> GetSongsByIdAsync(Guid id)
    {
        var song = await _songsRepository.FindByIdAsync(id)
            ?? throw new EntityNotFoundException("Song", id);
        return _mapper.Map<SongModel>(song);
    }

    public async Task<IEnumerable<SongModel>> GetSongByNameAsync(string titlePart)
    {
        var songs = await _songsRepository.FindByTitlePartAsync(titlePart);
        return _mapper.Map<IEnumerable<SongModel>>(songs);
    }

    public async Task<SongModel> CreateSongAsync(CreateSongModel model) // TODO: проверку согласованности имён
    {
        var song = _mapper.Map<Song>(model);
        song = await _songsRepository.SaveAsync(song)
               ?? throw new EntityAlreadyExistsException("song");
        return _mapper.Map<SongModel>(song);
    }

    public async Task<SongModel> DeleteSongAsync(Guid id)
    {
        var song = await _songsRepository.FindByIdAsync(id)
            ?? throw new EntityNotFoundException("Song", id);
        await _songsRepository.DeleteAsync(song);
        return _mapper.Map<SongModel>(song);
    }

    public async Task<SongModel> UpdateSongAsync(UpdateSongModel model, Guid id)
    {
        var song = await _songsRepository.FindByIdAsync(id)
                   ?? throw new EntityNotFoundException("Song", id);
        song.Title = model?.Title ?? song.Title;
        song.Duration = model?.Duration ?? song.Duration;
        song.TrackNumber = model?.TrackNumber ?? song.TrackNumber;
        if (model.AlbumTitle is not null)
        {
            song.Album = await _albumsRepository.FindByTitleAsync(model.Title)
                ?? throw new EntityNotFoundException("Album");
        }

        if (model.Artists is not null)
        {
            var artists = new List<Artist>();
            foreach (var name in model.Artists)
            {
                var artist = await _artistsRepository.FindByNameAsync(name)
                    ?? throw new EntityNotFoundException("Artist");
                artists.Add(artist);
            }

            if (!song.Album.Artists.Intersect(artists).Any())
            {
                throw new WrongArtistNameConsistencyException();
            }
            
            song.Artists = artists;
        }

        song = await _songsRepository.UpdateAsync(song);
        return _mapper.Map<SongModel>(song);
    }
}