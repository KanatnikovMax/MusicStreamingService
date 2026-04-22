import React from 'react';
import { Link } from 'react-router-dom';
import { ListMusic, Play, Trash2 } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import { useMusicPlayer } from '../contexts/MusicPlayerContext';
import { useToast } from '../contexts/ToastContext';
import { deletePlaylist, getPlaylistSongs } from '../services/playlistService';
import type { Playlist } from '../types/music';

interface PlaylistCardProps {
  playlist: Playlist;
  onPlaylistDeleted?: (playlistId: string) => void;
}

const PlaylistCard: React.FC<PlaylistCardProps> = ({ playlist, onPlaylistDeleted }) => {
  const { isAuthenticated, user, isAdmin } = useAuth();
  const { setCurrentSong } = useMusicPlayer();
  const { showToast } = useToast();

  const handlePlayPlaylist = async (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
    event.stopPropagation();

    if (!user) {
      return;
    }

    try {
      const response = await getPlaylistSongs(user.id, playlist.id, { pageSize: 100 });
      if (response.items.length === 0) {
        showToast('Playlist is empty', 'info');
        return;
      }

      setCurrentSong(response.items[0]);
    } catch {
      showToast('Failed to play playlist', 'error');
    }
  };

  const handleDeletePlaylist = async (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
    event.stopPropagation();

    if (!user) {
      return;
    }

    try {
      await deletePlaylist(user.id, playlist.id);
      showToast('Playlist deleted', 'success');
      onPlaylistDeleted?.(playlist.id);
    } catch {
      showToast('Failed to delete playlist', 'error');
    }
  };

  return (
      <div className="overflow-hidden rounded-lg bg-white shadow-md transition-transform hover:-translate-y-1 hover:shadow-lg">
        <Link to={`/playlists/${playlist.id}`}>
          <div className="group relative aspect-square bg-gradient-to-br from-indigo-500 via-indigo-600 to-fuchsia-600">
            <div className="flex h-full items-center justify-center">
              <ListMusic size={72} className="text-white/90" />
            </div>
            <div className="absolute inset-0 flex items-center justify-center bg-black bg-opacity-0 transition-opacity group-hover:bg-opacity-30">
              <button
                  onClick={handlePlayPlaylist}
                  className="rounded-full bg-white p-3 text-indigo-700 opacity-0 transition-opacity hover:scale-105 group-hover:opacity-100"
              >
                <Play size={24} />
              </button>
            </div>
          </div>
        </Link>

        <div className="p-4">
          <Link to={`/playlists/${playlist.id}`} className="block">
            <h3 className="truncate text-lg font-semibold text-gray-900 hover:text-indigo-600">
              {playlist.name}
            </h3>
          </Link>

          <div className="mt-1 text-sm text-gray-500">Personal playlist</div>

          {isAuthenticated && !isAdmin && (
              <div className="mt-3 flex justify-end">
                <button
                    onClick={handleDeletePlaylist}
                    className="rounded-full p-2 text-red-600 hover:bg-red-100 hover:text-red-800"
                    aria-label={`Delete playlist ${playlist.name}`}
                >
                  <Trash2 size={18} />
                </button>
              </div>
          )}
        </div>
      </div>
  );
};

export default PlaylistCard;
