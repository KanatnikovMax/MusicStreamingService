import React, { useEffect, useState } from 'react';
import { ArrowLeft, ListMusic, Play, Trash2 } from 'lucide-react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import TrackList from '../../components/TrackList';
import { useAuth } from '../../contexts/AuthContext';
import { useMusicPlayer } from '../../contexts/MusicPlayerContext';
import { useToast } from '../../contexts/ToastContext';
import {
  deletePlaylist,
  getPlaylistSongs,
  getUserPlaylistById,
  removeSongFromPlaylist
} from '../../services/playlistService';
import { getUserSongs } from '../../services/userService';
import type { Playlist, Song } from '../../types/music';

const PlaylistDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [playlist, setPlaylist] = useState<Playlist | null>(null);
  const [songs, setSongs] = useState<Song[]>([]);
  const [userSavedSongs, setUserSavedSongs] = useState<string[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const { user, isAuthenticated } = useAuth();
  const { setCurrentSong } = useMusicPlayer();
  const { showToast } = useToast();

  useEffect(() => {
    const fetchPlaylist = async () => {
      if (!id || !user) {
        return;
      }

      setIsLoading(true);
      try {
        const [playlistData, songsData] = await Promise.all([
          getUserPlaylistById(user.id, id),
          getPlaylistSongs(user.id, id, { pageSize: 100 })
        ]);

        setPlaylist(playlistData);
        setSongs(songsData.items);

        if (isAuthenticated) {
          const userSongs = await getUserSongs(user.id, { pageSize: 100 });
          setUserSavedSongs(userSongs.items.map(song => song.id));
        }
      } catch {
        showToast('Failed to load playlist', 'error');
      } finally {
        setIsLoading(false);
      }
    };

    fetchPlaylist();
  }, [id, isAuthenticated, showToast, user]);

  const handlePlayPlaylist = () => {
    if (songs.length > 0) {
      setCurrentSong(songs[0]);
    }
  };

  const handleDeletePlaylist = async () => {
    if (!user || !playlist) {
      return;
    }

    try {
      await deletePlaylist(user.id, playlist.id);
      showToast('Playlist deleted', 'success');
      navigate('/playlists');
    } catch {
      showToast('Failed to delete playlist', 'error');
    }
  };

  const handleRemoveSongFromPlaylist = async (songId: string) => {
    if (!user || !playlist) {
      return;
    }

    try {
      await removeSongFromPlaylist(user.id, playlist.id, songId);
      setSongs(prev => prev.filter(song => song.id !== songId));
      showToast('Song removed from playlist', 'success');
    } catch {
      showToast('Failed to remove song from playlist', 'error');
    }
  };

  const handleSongSaved = (songId: string) => {
    setUserSavedSongs(prev => [...prev, songId]);
  };

  const handleSongRemoved = (songId: string) => {
    setUserSavedSongs(prev => prev.filter(id => id !== songId));
  };

  if (!isAuthenticated) {
    return (
        <div className="flex h-64 flex-col items-center justify-center">
          <h2 className="mb-2 text-xl font-semibold text-gray-900">Please login to view your playlists</h2>
          <p className="text-gray-500">Playlist management is available only for authorized users.</p>
        </div>
    );
  }

  if (isLoading) {
    return (
        <div className="flex h-64 items-center justify-center">
          <div className="h-12 w-12 animate-spin rounded-full border-b-2 border-t-2 border-indigo-500" />
        </div>
    );
  }

  if (!playlist) {
    return (
        <div className="py-10 text-center">
          <h2 className="text-xl font-semibold text-gray-900">Playlist not found</h2>
          <Link to="/playlists" className="mt-4 inline-flex items-center text-indigo-600 hover:text-indigo-500">
            <ArrowLeft size={16} className="mr-1" /> Back to Playlists
          </Link>
        </div>
    );
  }

  return (
      <div>
        <Link
            to="/playlists"
            className="mb-6 inline-flex items-center text-indigo-600 hover:text-indigo-500"
        >
          <ArrowLeft size={16} className="mr-1" /> Back to Playlists
        </Link>

        <div className="mb-8 overflow-hidden rounded-lg bg-white shadow">
          <div className="md:flex">
            <div className="p-6 md:w-1/3">
              <div className="mb-4 aspect-square overflow-hidden rounded-md">
                {playlist.photoUrl ? (
                    <img
                        src={playlist.photoUrl}
                        alt={playlist.name}
                        className="h-full w-full object-cover"
                    />
                ) : (
                    <div className="flex h-full items-center justify-center bg-gradient-to-br from-indigo-500 via-indigo-600 to-fuchsia-600">
                      <ListMusic size={96} className="text-white/90" />
                    </div>
                )}
              </div>

              <h1 className="mb-2 text-2xl font-bold text-gray-900">{playlist.name}</h1>

              <div className="mb-6 text-sm text-gray-500">
                <p>Personal playlist</p>
                <p>{songs.length} songs</p>
              </div>

              <div className="flex space-x-2">
                <button
                    onClick={handlePlayPlaylist}
                    className="rounded-md bg-indigo-600 px-4 py-2 text-white transition-colors hover:bg-indigo-700"
                >
                  <span className="inline-flex items-center">
                    <Play size={16} className="mr-1" />
                    Play
                  </span>
                </button>
                <button
                    onClick={handleDeletePlaylist}
                    className="inline-flex items-center rounded-md border border-gray-300 px-4 py-2 transition-colors hover:bg-gray-50"
                >
                  <Trash2 size={16} className="mr-1 text-red-600" />
                  Delete
                </button>
              </div>
            </div>

            <div className="md:w-2/3">
              <div className="border-t border-gray-200 md:border-l md:border-t-0">
                <h2 className="border-b border-gray-200 p-4 text-xl font-semibold">Songs</h2>

                {songs.length > 0 ? (
                    <TrackList
                        songs={songs}
                        userSavedSongs={userSavedSongs}
                        onSongSaved={handleSongSaved}
                        onSongRemoved={handleSongRemoved}
                        extraActionLabel="Remove"
                        renderExtraAction={(song) => (
                            <button
                                onClick={() => handleRemoveSongFromPlaylist(song.id)}
                                className="rounded-full p-2 text-red-600 hover:bg-red-100 hover:text-red-800"
                                aria-label={`Remove ${song.title} from playlist`}
                            >
                              <Trash2 size={16} />
                            </button>
                        )}
                    />
                ) : (
                    <div className="p-6 text-center text-gray-500">
                      No songs in this playlist yet.
                    </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
  );
};

export default PlaylistDetailPage;
