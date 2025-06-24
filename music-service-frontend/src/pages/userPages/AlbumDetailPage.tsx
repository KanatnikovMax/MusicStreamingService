import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { ArrowLeft, Plus, Check } from 'lucide-react';
import { Link } from 'react-router-dom';
import TrackList from '../../components/TrackList.tsx';
import { getAlbumById, getAlbumSongs } from '../../services/albumService.ts';
import { useToast } from '../../contexts/ToastContext.tsx';
import { useMusicPlayer } from '../../contexts/MusicPlayerContext.tsx';
import { useAuth } from '../../contexts/AuthContext.tsx';
import { addAlbumToAccount, deleteAlbumFromAccount, getUserSongs } from '../../services/userService.ts';
import type {Album, Song} from '../../types/music.ts';

const AlbumDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [album, setAlbum] = useState<Album | null>(null);
  const [songs, setSongs] = useState<Song[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaved, setIsSaved] = useState(false);
  const [userSavedSongs, setUserSavedSongs] = useState<string[]>([]);

  const { showToast } = useToast();
  const { setCurrentSong } = useMusicPlayer();
  const { isAuthenticated, user } = useAuth();

  useEffect(() => {
    const fetchAlbum = async () => {
      if (!id) return;

      setIsLoading(true);
      try {
        const albumData = await getAlbumById(id);
        setAlbum(albumData.albums[0]);

        const songsData = await getAlbumSongs(id);
        setSongs(songsData.items);

        if (isAuthenticated && user) {
          const userSongs = await getUserSongs(user.id, { pageSize: 100 });
          setUserSavedSongs(userSongs.items.map(song => song.id));
        }

      } catch {
        showToast('Failed to load album', 'error');
      } finally {
        setIsLoading(false);
      }
    };

    fetchAlbum();
  }, [id, showToast, isAuthenticated, user]);

  const handlePlayAlbum = () => {
    if (songs.length > 0) {
      setCurrentSong(songs[0]);
    }
  };

  const handleSaveAlbum = async () => {
    if (!isAuthenticated || !user || !id) {
      showToast('Please login to save albums', 'info');
      return;
    }

    try {
      await addAlbumToAccount(user.id, id);
      setIsSaved(true);
      showToast('Album added to your library', 'success');
    } catch {
      showToast('Failed to add album', 'error');
    }
  };

  const handleRemoveAlbum = async () => {
    if (!isAuthenticated || !user || !id) return;

    try {
      await deleteAlbumFromAccount(user.id, id);
      setIsSaved(false);
      showToast('Album removed from your library', 'success');
    } catch {
      showToast('Failed to remove album', 'error');
    }
  };

  const handleSongSaved = (songId: string) => {
    setUserSavedSongs(prev => [...prev, songId]);
  };

  const handleSongRemoved = (songId: string) => {
    setUserSavedSongs(prev => prev.filter(id => id !== songId));
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString();
  };

  if (isLoading) {
    return (
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
        </div>
    );
  }

  if (!album) {
    return (
        <div className="text-center py-10">
          <h2 className="text-xl font-semibold text-gray-900">Album not found</h2>
          <Link to="/albums" className="mt-4 inline-flex items-center text-indigo-600 hover:text-indigo-500">
            <ArrowLeft size={16} className="mr-1" /> Back to Albums
          </Link>
        </div>
    );
  }

  return (
      <div>
        <Link
            to="/albums"
            className="inline-flex items-center text-indigo-600 hover:text-indigo-500 mb-6"
        >
          <ArrowLeft size={16} className="mr-1" /> Back to Albums
        </Link>

        <div className="bg-white shadow rounded-lg overflow-hidden mb-8">
          <div className="md:flex">
            <div className="md:w-1/3 p-6">
              <div className="aspect-square rounded-md mb-4 overflow-hidden">
                {album.photoBase64 ? (
                    <img
                        src={`data:image/jpeg;base64,${album.photoBase64}`}
                        alt={album.title}
                        className="w-full h-full object-cover"
                    />
                ) : (
                    <div className="w-full h-full bg-gray-200 border-2 border-dashed rounded-xl flex items-center justify-center">
                      <div className="bg-gray-300 border-2 border-dashed rounded-xl w-16 h-16" />
                    </div>
                )}
              </div>

              <h1 className="text-2xl font-bold text-gray-900 mb-2">{album.title}</h1>

              <div className="text-sm text-gray-500 mb-4">
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

              <div className="text-sm text-gray-500 mb-6">
                <p>Released: {formatDate(album.releaseDate)}</p>
                <p>{songs.length} songs</p>
              </div>

              <div className="flex space-x-2">
                <button
                    onClick={handlePlayAlbum}
                    className="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 transition-colors"
                >
                  Play
                </button>

                {isAuthenticated && (
                    isSaved ? (
                        <button
                            onClick={handleRemoveAlbum}
                            className="px-4 py-2 flex items-center border border-gray-300 rounded-md hover:bg-gray-50 transition-colors"
                        >
                          <Check size={16} className="mr-1 text-green-600" /> Saved
                        </button>
                    ) : (
                        <button
                            onClick={handleSaveAlbum}
                            className="px-4 py-2 flex items-center border border-gray-300 rounded-md hover:bg-gray-50 transition-colors"
                        >
                          <Plus size={16} className="mr-1" /> Save
                        </button>
                    )
                )}
              </div>
            </div>

            <div className="md:w-2/3">
              <div className="border-t md:border-t-0 md:border-l border-gray-200">
                <h2 className="text-xl font-semibold p-4 border-b border-gray-200">Songs</h2>

                {songs.length > 0 ? (
                    <TrackList
                        songs={songs}
                        userSavedSongs={userSavedSongs}
                        onSongSaved={handleSongSaved}
                        onSongRemoved={handleSongRemoved}
                    />
                ) : (
                    <div className="p-6 text-center text-gray-500">
                      No songs available for this album.
                    </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
  );
};

export default AlbumDetailPage;