import React, { useState, useEffect, useCallback, useRef } from 'react';
import { useParams } from 'react-router-dom';
import { ArrowLeft, Search } from 'lucide-react';
import { Link } from 'react-router-dom';
import AlbumCard from '../../components/AlbumCard.tsx';
import TrackList from '../../components/TrackList.tsx';
import {
  getArtistById,
  getArtistAlbums,
  getArtistSongs,
  getArtistSongsByTitle
} from '../../services/artistService.ts';
import { getUserAlbums, getUserSongs } from '../../services/userService.ts';
import { useToast } from '../../contexts/ToastContext.tsx';
import { useAuth } from '../../contexts/AuthContext.tsx';
import type {Artist, Album, Song} from '../../types/music.ts';

const ArtistDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [artist, setArtist] = useState<Artist | null>(null);
  const [albums, setAlbums] = useState<Album[]>([]);
  const [songs, setSongs] = useState<Song[]>([]);
  const [isLoading, setIsLoading] = useState({ main: true, songs: false });
  const [userSavedAlbums, setUserSavedAlbums] = useState<string[]>([]);
  const [userSavedSongs, setUserSavedSongs] = useState<string[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [hasMoreSongs, setHasMoreSongs] = useState(true);
  const [songsCursor, setSongsCursor] = useState<Date>();
  const [isInitialized, setIsInitialized] = useState(false);

  const { showToast } = useToast();
  const { isAuthenticated, user } = useAuth();
  const observer = useRef<IntersectionObserver | null>(null);
  const lastSongRef = useRef<HTMLDivElement>(null);
  const abortControllerRef = useRef<AbortController | null>(null);

  const pageSize = 20;

  const fetchInitialData = useCallback(async () => {
    if (!id) return;

    try {
      setIsLoading(prev => ({ ...prev, main: true }));

      const [artistData, albumsData] = await Promise.all([
        getArtistById(id),
        getArtistAlbums(id, { pageSize: 100 })
      ]);

      setArtist(artistData.artists[0]);
      setAlbums(albumsData.items);

      if (isAuthenticated && user) {
        const [userAlbums, userSongs] = await Promise.all([
          getUserAlbums(user.id, { pageSize: 100 }),
          getUserSongs(user.id, { pageSize: 100 })
        ]);

        setUserSavedAlbums(userAlbums.items.map(album => album.id));
        setUserSavedSongs(userSongs.items.map(song => song.id));
      }
    } catch {
      showToast('Failed to load artist', 'error');
    } finally {
      setIsLoading(prev => ({ ...prev, main: false }));
    }
  }, [id, showToast, isAuthenticated, user]);

  const fetchSongs = useCallback(async (loadMore = false) => {
    if (!id || isLoading.songs || (loadMore && !hasMoreSongs)) return;

    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
    }
    const controller = new AbortController();
    abortControllerRef.current = controller;

    try {
      setIsLoading(prev => ({ ...prev, songs: true }));

      const response = searchTerm
          ? await getArtistSongsByTitle(id, searchTerm, {
            cursor: loadMore ? songsCursor : undefined,
            pageSize
          })
          : await getArtistSongs(id, {
            cursor: loadMore ? songsCursor : undefined,
            pageSize
          });

      setSongs(prev => loadMore
          ? [...prev, ...response.items.filter(item => !prev.some(p => p.id === item.id))]
          : response.items
      );

      setHasMoreSongs(!!response.cursor);
      setSongsCursor(response.cursor);
      setIsInitialized(true);
    } catch {
      setHasMoreSongs(false);
    } finally {
      if (!controller.signal.aborted) {
        setIsLoading(prev => ({ ...prev, songs: false }));
      }
    }
  }, [id, searchTerm, songsCursor, pageSize, hasMoreSongs]);

  useEffect(() => {
    fetchInitialData();
  }, [fetchInitialData]);

  useEffect(() => {
    const handler = setTimeout(() => {
      setSongs([]);
      setSongsCursor(undefined);
      setHasMoreSongs(true);
      fetchSongs(false);
    }, 500);

    return () => {
      clearTimeout(handler);
      if (abortControllerRef.current) {
        abortControllerRef.current.abort();
      }
    };
  }, [searchTerm]);

  useEffect(() => {
    const callback = (entries: IntersectionObserverEntry[]) => {
      if (entries[0]?.isIntersecting && hasMoreSongs && !isLoading.songs) {
        fetchSongs(true);
      }
    };

    observer.current = new IntersectionObserver(callback, {
      root: null,
      rootMargin: '100px',
      threshold: 0.1
    });

    if (lastSongRef.current) {
      observer.current.observe(lastSongRef.current);
    }

    return () => observer.current?.disconnect();
  }, [hasMoreSongs, isLoading.songs, songs]);

  const handleAlbumSaved = (albumId: string) => {
    setUserSavedAlbums(prev => [...prev, albumId]);
  };

  const handleAlbumRemoved = (albumId: string) => {
    setUserSavedAlbums(prev => prev.filter(id => id !== albumId));
  };

  const handleSongSaved = (songId: string) => {
    setUserSavedSongs(prev => [...prev, songId]);
  };

  const handleSongRemoved = (songId: string) => {
    setUserSavedSongs(prev => prev.filter(id => id !== songId));
  };

  if (isLoading.main) {
    return (
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
        </div>
    );
  }

  if (!artist) {
    return (
        <div className="text-center py-10">
          <h2 className="text-xl font-semibold text-gray-900">Artist not found</h2>
          <Link to="/artists" className="mt-4 inline-flex items-center text-indigo-600 hover:text-indigo-500">
            <ArrowLeft size={16} className="mr-1" /> Back to Artists
          </Link>
        </div>
    );
  }

  return (
      <div>
        <Link
            to="/artists"
            className="inline-flex items-center text-indigo-600 hover:text-indigo-500 mb-6"
        >
          <ArrowLeft size={16} className="mr-1" /> Back to Artists
        </Link>

        <div className="bg-white shadow rounded-lg overflow-hidden mb-8">
          <div className="bg-gradient-to-r from-purple-800 to-indigo-800 text-white p-8">
            <div className="flex items-center">
              <div className="h-24 w-24 rounded-full overflow-hidden bg-white bg-opacity-20 flex items-center justify-center">
                {artist.photoBase64 ? (
                    <img
                        src={`data:image/jpeg;base64,${artist.photoBase64}`}
                        alt={artist.name}
                        className="w-full h-full object-cover"
                    />
                ) : (
                    <span className="text-3xl font-bold">{artist.name.charAt(0)}</span>
                )}
              </div>

              <div className="ml-6">
                <h1 className="text-3xl font-bold">{artist.name}</h1>
                <p className="text-indigo-200 mt-1">
                  {albums.length} {albums.length === 1 ? 'Album' : 'Albums'} • {songs.length} {songs.length === 1 ? 'Song' : 'Songs'}
                </p>
              </div>
            </div>
          </div>

          <div className="p-6">
            <h2 className="text-xl font-semibold mb-4">Albums</h2>

            {albums.length > 0 ? (
                <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 mb-8">
                  {albums.map(album => (
                      <AlbumCard
                          key={album.id}
                          album={album}
                          isSaved={userSavedAlbums.includes(album.id)}
                          onAlbumSaved={handleAlbumSaved}
                          onAlbumRemoved={handleAlbumRemoved}
                      />
                  ))}
                </div>
            ) : (
                <div className="text-center text-gray-500 mb-8">
                  No albums available for this artist.
                </div>
            )}

            <div className="border-t pt-6">
              <div className="flex justify-between items-center mb-4">
                <h2 className="text-xl font-semibold">Songs</h2>

                <div className="relative w-64">
                  <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                    <Search size={16} className="text-gray-400" />
                  </div>
                  <input
                      type="text"
                      className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                      placeholder="Search songs..."
                      value={searchTerm}
                      onChange={(e) => setSearchTerm(e.target.value)}
                  />
                </div>
              </div>

              {!isInitialized || isLoading.songs ? (
                  <div className="p-8 flex justify-center">
                    <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500" />
                  </div>
              ) : songs.length === 0 ? (
                  <div className="text-center text-gray-500">
                    {searchTerm ? 'No songs match your search' : 'No songs available for this artist.'}
                  </div>
              ) : (
                  <div className="divide-y divide-gray-200">
                    <TrackList
                        songs={songs}
                        userSavedSongs={userSavedSongs}
                        onSongSaved={handleSongSaved}
                        onSongRemoved={handleSongRemoved}
                    />
                    <div ref={lastSongRef} />
                    {isLoading.songs && (
                        <div className="p-4 flex justify-center">
                          <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-indigo-500" />
                        </div>
                    )}
                  </div>
              )}
            </div>
          </div>
        </div>
      </div>
  );
};

export default ArtistDetailPage;