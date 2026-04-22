import React from 'react';
import { Play, Pause, Plus, Check, Clock, ListPlus } from 'lucide-react';
import { useMusicPlayer } from '../contexts/MusicPlayerContext';
import { useAuth } from '../contexts/AuthContext';
import type { Playlist, Song } from '../types/music';
import { addSongToAccount, deleteSongFromAccount } from '../services/userService';
import { useToast } from '../contexts/ToastContext';
import { Link } from 'react-router-dom';
import AddSongToPlaylistModal from './AddSongToPlaylistModal';
import { addSongToPlaylist, getUserPlaylists } from '../services/playlistService';

interface TrackListProps {
  songs: Song[];
  userSavedSongs?: string[];
  onSongSaved?: (songId: string) => void;
  onSongRemoved?: (songId: string) => void;
  extraActionLabel?: string;
  renderExtraAction?: (song: Song) => React.ReactNode;
}

const TrackList: React.FC<TrackListProps> = ({
  songs,
  userSavedSongs = [],
  onSongSaved,
  onSongRemoved,
  extraActionLabel,
  renderExtraAction
}) => {
  const {
    currentSong,
    isPlaying,
    setCurrentSong,
    togglePlay
  } = useMusicPlayer();

  const { isAuthenticated, user, isAdmin } = useAuth();
  const { showToast } = useToast();
  const [playlists, setPlaylists] = React.useState<Playlist[]>([]);
  const [isLoadingPlaylists, setIsLoadingPlaylists] = React.useState(false);
  const [selectedSong, setSelectedSong] = React.useState<Song | null>(null);

  const formatDuration = (seconds: number) => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
  };

  const handlePlayPause = (song: Song) => {
    if (currentSong && currentSong.id === song.id) {
      togglePlay();
    } else {
      setCurrentSong(song);
    }
  };

  const handleAddSong = async (songId: string) => {
    if (!isAuthenticated || !user) {
      showToast('Please login to save songs', 'info');
      return;
    }

    try {
      await addSongToAccount(user.id, songId);
      showToast('Song added to your library', 'success');
      if (onSongSaved) onSongSaved(songId);
    } catch {
      showToast('Failed to add song', 'error');
    }
  };

  const handleRemoveSong = async (songId: string) => {
    if (!isAuthenticated || !user) return;

    try {
      await deleteSongFromAccount(user.id, songId);
      showToast('Song removed from your library', 'success');
      if (onSongRemoved) onSongRemoved(songId);
    } catch {
      showToast('Failed to remove song', 'error');
    }
  };

  const isSongSaved = (songId: string) => {
    return userSavedSongs.includes(songId);
  };

  const fetchPlaylists = React.useCallback(async () => {
    if (!isAuthenticated || !user || isAdmin) {
      return;
    }

    try {
      setIsLoadingPlaylists(true);
      const response = await getUserPlaylists(user.id, { pageSize: 100 });
      setPlaylists(response.items);
    } catch {
      showToast('Failed to load playlists', 'error');
    } finally {
      setIsLoadingPlaylists(false);
    }
  }, [isAuthenticated, user, isAdmin, showToast]);

  React.useEffect(() => {
    fetchPlaylists();
  }, [fetchPlaylists]);

  const handleOpenPlaylistModal = async (song: Song) => {
    if (!isAuthenticated || !user) {
      showToast('Please login to manage playlists', 'info');
      return;
    }

    setSelectedSong(song);
    if (playlists.length === 0) {
      await fetchPlaylists();
    }
  };

  const handleAddToPlaylist = async (playlistId: string) => {
    if (!selectedSong || !user) {
      return;
    }

    try {
      await addSongToPlaylist(user.id, playlistId, selectedSong.id);
      showToast('Song added to playlist', 'success');
      setSelectedSong(null);
    } catch {
      showToast('Failed to add song to playlist', 'error');
    }
  };

  return (
      <>
      <div className="overflow-x-auto">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
          <tr>
            <th scope="col" className="px-2 w-10"></th>
            <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Title
            </th>
            <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Artists
            </th>
            <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              <Clock size={16} />
            </th>
            {isAuthenticated && !isAdmin && (
                <th scope="col" className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Save
                </th>
            )}
            {isAuthenticated && !isAdmin && (
                <th scope="col" className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Playlist
                </th>
            )}
            {renderExtraAction && extraActionLabel && (
                <th scope="col" className="px-6 py-3 text-right text-xs font-medium uppercase tracking-wider text-gray-500">
                  {extraActionLabel}
                </th>
            )}
          </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
          {songs.map((song) => (
              <tr key={song.id} className="hover:bg-gray-50">
                <td className="px-2">
                  <button
                      onClick={() => handlePlayPause(song)}
                      className="p-2 text-indigo-600 hover:text-indigo-900 rounded-full hover:bg-indigo-100"
                  >
                    {currentSong && currentSong.id === song.id && isPlaying ? (
                        <Pause size={16} />
                    ) : (
                        <Play size={16} />
                    )}
                  </button>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <div className="text-sm font-medium text-gray-900">
                    <Link
                        to={`/albums/${song.albumId}`}
                        className="hover:text-indigo-600"
                    >
                      {song.title}
                    </Link>
                  </div>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {song.artists.map((artist, index) => (
                      <React.Fragment key={artist.id}>
                        <Link
                            to={`/artists/${artist.id}`}
                            className="hover:text-indigo-600"
                        >
                          {artist.name}
                        </Link>
                        {index < song.artists.length - 1 && ", "}
                      </React.Fragment>
                  ))}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {formatDuration(song.duration)}
                </td>
                {isAuthenticated && !isAdmin && (
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      {isSongSaved(song.id) ? (
                          <button
                              onClick={() => handleRemoveSong(song.id)}
                              className="text-green-600 hover:text-green-900 p-2 rounded-full hover:bg-green-100"
                          >
                            <Check size={16} />
                          </button>
                      ) : (
                          <button
                              onClick={() => handleAddSong(song.id)}
                              className="text-indigo-600 hover:text-indigo-900 p-2 rounded-full hover:bg-indigo-100"
                          >
                            <Plus size={16} />
                          </button>
                      )}
                    </td>
                )}
                {isAuthenticated && !isAdmin && (
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <button
                          onClick={() => handleOpenPlaylistModal(song)}
                          className="text-indigo-600 hover:text-indigo-900 p-2 rounded-full hover:bg-indigo-100"
                          aria-label={`Add ${song.title} to playlist`}
                      >
                        <ListPlus size={16} />
                      </button>
                    </td>
                )}
                {renderExtraAction && (
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      {renderExtraAction(song)}
                    </td>
                )}
              </tr>
          ))}
          </tbody>
        </table>
      </div>
        <AddSongToPlaylistModal
            isOpen={!!selectedSong}
            playlists={playlists}
            isLoading={isLoadingPlaylists}
            songTitle={selectedSong?.title ?? ''}
            onClose={() => setSelectedSong(null)}
            onSelectPlaylist={handleAddToPlaylist}
        />
      </>
  );
};

export default TrackList;
