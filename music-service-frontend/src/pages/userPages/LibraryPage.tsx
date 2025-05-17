import React, { useState, useEffect, useCallback, useRef } from 'react';
import { Search } from 'lucide-react';
import { getUserAlbums, getUserSongs, getUserAlbumsByTitle, getUserSongsByName } from '../../services/userService.ts';
import { useAuth } from '../../contexts/AuthContext.tsx';
import AlbumCard from '../../components/AlbumCard.tsx';
import TrackList from '../../components/TrackList.tsx';
import type { Album, Song } from '../../types/music.ts';

const LibraryPage: React.FC = () => {
  const [savedAlbums, setSavedAlbums] = useState<Album[]>([]);
  const [savedSongs, setSavedSongs] = useState<Song[]>([]);
  const [isLoading, setIsLoading] = useState({ songs: false, albums: false });
  const [isInitialized, setIsInitialized] = useState({ songs: false, albums: false });
  const [activeTab, setActiveTab] = useState<'songs' | 'albums'>('songs');
  const [hasMore, setHasMore] = useState({ songs: true, albums: true });
  const [cursor, setCursor] = useState<{ songs?: Date; albums?: Date }>({});
  const [searchTerms, setSearchTerms] = useState({ songs: '', albums: '' });

  const { user, isAuthenticated } = useAuth();
  const observer = useRef<IntersectionObserver | null>(null);
  const lastItemRef = useRef<HTMLDivElement>(null);
  const abortControllerRef = useRef<AbortController | null>(null);

  const pageSize = 15;

  const fetchUserLibrary = useCallback(async (loadMore = false) => {
    if (!isAuthenticated || !user) return;

    const currentTab = activeTab;
    const currentCursor = cursor[currentTab];
    const currentSearchTerm = searchTerms[currentTab];

    if (isLoading[currentTab] || (loadMore && !hasMore[currentTab])) return;

    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
    }
    const controller = new AbortController();
    abortControllerRef.current = controller;

    try {
      setIsLoading(prev => ({ ...prev, [currentTab]: true }));

      if (currentTab === 'songs') {
        const response = currentSearchTerm
            ? await getUserSongsByName(user.id, currentSearchTerm, {
              cursor: loadMore ? currentCursor : undefined,
              pageSize
            })
            : await getUserSongs(user.id, {
              cursor: loadMore ? currentCursor : undefined,
              pageSize
            });

        setSavedSongs(prev => loadMore
            ? [...prev, ...response.items.filter(item => !prev.some(p => p.id === item.id))]
            : response.items
        );
        setHasMore(prev => ({ ...prev, songs: !!response.cursor }));
        setCursor(prev => ({ ...prev, songs: response.cursor }));
      } else {
        const response = currentSearchTerm
            ? await getUserAlbumsByTitle(user.id, currentSearchTerm, {
              cursor: loadMore ? currentCursor : undefined,
              pageSize
            })
            : await getUserAlbums(user.id, {
              cursor: loadMore ? currentCursor : undefined,
              pageSize
            });

        setSavedAlbums(prev => loadMore
            ? [...prev, ...response.items.filter(item => !prev.some(p => p.id === item.id))]
            : response.items
        );
        setHasMore(prev => ({ ...prev, albums: !!response.cursor }));
        setCursor(prev => ({ ...prev, albums: response.cursor }));
      }
    } catch {
      setHasMore(prev => ({...prev, [currentTab]: false}));
    } finally {
      if (!controller.signal.aborted) {
        setIsLoading(prev => ({ ...prev, [currentTab]: false }));
        setIsInitialized(prev => ({ ...prev, [currentTab]: true }));
      }
    }
  }, [user, isAuthenticated, activeTab, cursor, hasMore, searchTerms, pageSize]);

  useEffect(() => {
    const handler = setTimeout(() => {
      if (activeTab === 'songs') {
        setSavedSongs([]);
        setCursor(prev => ({ ...prev, songs: undefined }));
        setHasMore(prev => ({ ...prev, songs: true }));
      } else {
        setSavedAlbums([]);
        setCursor(prev => ({ ...prev, albums: undefined }));
        setHasMore(prev => ({ ...prev, albums: true }));
      }
      setIsInitialized(prev => ({ ...prev, [activeTab]: false }));
      fetchUserLibrary(false);
    }, 300);

    return () => {
      clearTimeout(handler);
      if (abortControllerRef.current) {
        abortControllerRef.current.abort();
      }
    };
  }, [searchTerms[activeTab], activeTab]);

  useEffect(() => {
    const callback = (entries: IntersectionObserverEntry[]) => {
      if (entries[0].isIntersecting && hasMore[activeTab] && !isLoading[activeTab]) {
        fetchUserLibrary(true);
      }
    };

    observer.current = new IntersectionObserver(callback, {
      root: null,
      rootMargin: '100px',
      threshold: 0.1
    });

    if (lastItemRef.current) {
      observer.current.observe(lastItemRef.current);
    }

    return () => observer.current?.disconnect();
  }, [activeTab, hasMore, isLoading]);

  const handleSongRemoved = (songId: string) => {
    setSavedSongs(prev => prev.filter(song => song.id !== songId));
  };

  const handleAlbumRemoved = (albumId: string) => {
    setSavedAlbums(prev => prev.filter(album => album.id !== albumId));
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerms(prev => ({ ...prev, [activeTab]: e.target.value }));
  };

  if (!isAuthenticated) {
    return (
        <div className="flex flex-col items-center justify-center h-64">
          <h2 className="text-xl font-semibold text-gray-900 mb-2">Please login to view your library</h2>
          <p className="text-gray-500">Your saved songs and albums will appear here</p>
        </div>
    );
  }

  return (
      <div>
        <div className="mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Your Library</h1>
        </div>

        <div className="bg-white shadow rounded-lg overflow-hidden">
          <div className="border-b">
            <div className="flex space-x-6 px-6">
              <button
                  className={`pb-3 px-1 ${
                      activeTab === 'songs'
                          ? 'border-b-2 border-indigo-600 text-indigo-600 font-medium'
                          : 'text-gray-500 hover:text-gray-700'
                  }`}
                  onClick={() => setActiveTab('songs')}
              >
                Songs
              </button>
              <button
                  className={`pb-3 px-1 ${
                      activeTab === 'albums'
                          ? 'border-b-2 border-indigo-600 text-indigo-600 font-medium'
                          : 'text-gray-500 hover:text-gray-700'
                  }`}
                  onClick={() => setActiveTab('albums')}
              >
                Albums
              </button>
            </div>

            <div className="p-4 border-b">
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <Search size={20} className="text-gray-400" />
                </div>
                <input
                    type="text"
                    className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                    placeholder={activeTab === 'songs' ? 'Search songs or artists...' : 'Search albums...'}
                    value={searchTerms[activeTab]}
                    onChange={handleSearchChange}
                />
              </div>
            </div>
          </div>

          <div className="min-h-[400px]">
            {activeTab === 'songs' ? (
                !isInitialized.songs || isLoading.songs ? (
                    <div className="p-8 flex justify-center">
                      <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500" />
                    </div>
                ) : savedSongs.length === 0 ? (
                    <div className="text-center py-10">
                      <h3 className="text-lg font-medium text-gray-900 mb-1">
                        {searchTerms.songs ? 'No songs found' : 'No saved songs yet'}
                      </h3>
                      {!searchTerms.songs && (
                          <p className="text-gray-500">
                            Add songs to your library by clicking the + button next to a song
                          </p>
                      )}
                    </div>
                ) : (
                    <div className="divide-y divide-gray-200">
                      <TrackList
                          songs={savedSongs}
                          userSavedSongs={savedSongs.map(song => song.id)}
                          onSongRemoved={handleSongRemoved}
                      />
                      <div ref={lastItemRef} />
                      {isLoading.songs && (
                          <div className="p-4 flex justify-center">
                            <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-indigo-500" />
                          </div>
                      )}
                    </div>
                )
            ) : !isInitialized.albums || isLoading.albums ? (
                <div className="p-8 flex justify-center">
                  <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500" />
                </div>
            ) : savedAlbums.length === 0 ? (
                <div className="text-center py-10">
                  <h3 className="text-lg font-medium text-gray-900 mb-1">
                    {searchTerms.albums ? 'No albums found' : 'No saved albums yet'}
                  </h3>
                  {!searchTerms.albums && (
                      <p className="text-gray-500">
                        Add albums to your library by clicking the + button on an album
                      </p>
                  )}
                </div>
            ) : (
                <div className="p-6">
                  <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
                    {savedAlbums.map((album, index) => (
                        <div
                            key={album.id}
                            ref={index === savedAlbums.length - 1 ? lastItemRef : null}
                        >
                          <AlbumCard
                              album={album}
                              isSaved={true}
                              onAlbumRemoved={handleAlbumRemoved}
                          />
                        </div>
                    ))}
                  </div>
                  {isLoading.albums && (
                      <div className="col-span-full flex justify-center mt-4">
                        <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-indigo-500" />
                      </div>
                  )}
                </div>
            )}
          </div>
        </div>
      </div>
  );
};

export default LibraryPage;