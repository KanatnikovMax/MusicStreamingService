import React from 'react';
import { Link } from 'react-router-dom';
import { Plus, Check, Play } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import { useMusicPlayer } from '../contexts/MusicPlayerContext';
import { useToast } from '../contexts/ToastContext';
import { addAlbumToAccount, deleteAlbumFromAccount } from '../services/userService';
import type {Album} from '../types/music';

interface AlbumCardProps {
  album: Album;
  isSaved?: boolean;
  onAlbumSaved?: (albumId: string) => void;
  onAlbumRemoved?: (albumId: string) => void;
}

const AlbumCard: React.FC<AlbumCardProps> = ({
                                               album,
                                               isSaved = false,
                                               onAlbumSaved,
                                               onAlbumRemoved
                                             }) => {
  const { isAuthenticated, user, isAdmin } = useAuth();
  const { setCurrentSong, loadSongs } = useMusicPlayer();
  const { showToast } = useToast();

  const handlePlayAlbum = async () => {
    try {
      const songs = await loadSongs(album.id);
      if (songs.length > 0) {
        setCurrentSong(songs[0]);
      }
    } catch {
      showToast('Failed to play album', 'error');
    }
  };

  const handleAddAlbum = async () => {
    if (!isAuthenticated || !user) {
      showToast('Please login to save albums', 'info');
      return;
    }

    try {
      await addAlbumToAccount(user.id, album.id);
      showToast('Album added to your library', 'success');
      if (onAlbumSaved) onAlbumSaved(album.id);
    } catch {
      showToast('Failed to add album', 'error');
    }
  };

  const handleRemoveAlbum = async () => {
    if (!isAuthenticated || !user) return;

    try {
      await deleteAlbumFromAccount(user.id, album.id);
      showToast('Album removed from your library', 'success');
      if (onAlbumRemoved) onAlbumRemoved(album.id);
    } catch {
      showToast('Failed to remove album', 'error');
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  return (
      <div className="bg-white rounded-lg shadow-md overflow-hidden transition-transform hover:shadow-lg hover:-translate-y-1">
        <Link to={`/albums/${album.id}`}>
          <div className="aspect-square bg-gradient-to-br from-indigo-100 to-indigo-200 relative group">
            <div className="absolute inset-0 bg-black bg-opacity-0 group-hover:bg-opacity-30 transition-opacity flex items-center justify-center">
              <button
                  onClick={handlePlayAlbum}
                  className="p-3 bg-white text-indigo-700 rounded-full opacity-0 group-hover:opacity-100 transition-opacity transform hover:scale-105"
              >
                <Play size={24} />
              </button>
            </div>
          </div>
        </Link>

        <div className="p-4">
          <Link to={`/albums/${album.id}`} className="block">
            <h3 className="text-lg font-semibold text-gray-900 hover:text-indigo-600 truncate">{album.title}</h3>
          </Link>

          <div className="mt-1 text-sm text-gray-500">
            {album.artists.map((artist, index) => (
                <React.Fragment key={artist.id}>
                  <Link
                      to={`/artists/${artist.id}`}
                      className="hover:text-indigo-600"
                  >
                    {artist.name}
                  </Link>
                  {index < album.artists.length - 1 && ", "}
                </React.Fragment>
            ))}
          </div>

          <div className="mt-2 text-xs text-gray-500">
            {formatDate(album.releaseDate)}
          </div>

          {isAuthenticated && !isAdmin && (
              <div className="mt-3 flex justify-end">
                {isSaved ? (
                    <button
                        onClick={handleRemoveAlbum}
                        className="p-2 text-green-600 hover:text-green-800 rounded-full hover:bg-green-100"
                    >
                      <Check size={18} />
                    </button>
                ) : (
                    <button
                        onClick={handleAddAlbum}
                        className="p-2 text-indigo-600 hover:text-indigo-800 rounded-full hover:bg-indigo-100"
                    >
                      <Plus size={18} />
                    </button>
                )}
              </div>
          )}
        </div>
      </div>
  );
};

export default AlbumCard;