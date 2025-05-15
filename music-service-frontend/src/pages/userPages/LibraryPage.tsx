import React, { useState, useEffect, useCallback, useRef } from 'react';
import { getUserAlbums, getUserSongs } from '../../services/userService.ts';
import { useAuth } from '../../contexts/AuthContext.tsx';
import { useToast } from '../../contexts/ToastContext.tsx';
import AlbumCard from '../../components/AlbumCard.tsx';
import TrackList from '../../components/TrackList.tsx';
import type { Album, Song } from '../../types/music.ts';

const LibraryPage: React.FC = () => {
  const [savedAlbums, setSavedAlbums] = useState<Album[]>([]);
  const [savedSongs, setSavedSongs] = useState<Song[]>([]);
  const [isLoading, setIsLoading] = useState({ songs: false, albums: false });
  const [activeTab, setActiveTab] = useState<'songs' | 'albums'>('songs');
  const [hasMore, setHasMore] = useState({ songs: true, albums: true });
  const [cursor, setCursor] = useState<{ songs?: Date; albums?: Date }>({});

  const { user, isAuthenticated } = useAuth();
  const { showToast } = useToast();
  const observer = useRef<IntersectionObserver | null>(null);
  const lastItemRef = useRef<HTMLDivElement>(null);

  const pageSize = 15;

  const fetchUserLibrary = useCallback(async (loadMore = false) => {
    if (!isAuthenticated || !user) return;

    const currentTab = activeTab;
    const currentCursor = cursor[currentTab];

    if (isLoading[currentTab] || (loadMore && !hasMore[currentTab])) return;

    try {
      setIsLoading(prev => ({ ...prev, [currentTab]: true }));

      if (currentTab === 'songs') {
        const response = await getUserSongs(user.id, {
          cursor: loadMore ? currentCursor : undefined,
          pageSize
        });

        setSavedSongs(prev => loadMore
            ? [...prev, ...response.items.filter(item =>
                !prev.some(p => p.id === item.id)
            )]
            : response.items
        );
        setHasMore(prev => ({ ...prev, songs: !!response.cursor }));
        setCursor(prev => ({ ...prev, songs: response.cursor }));
      } else {
        const response = await getUserAlbums(user.id, {
          cursor: loadMore ? currentCursor : undefined,
          pageSize
        });

        setSavedAlbums(prev => loadMore
            ? [...prev, ...response.items.filter(item =>
                !prev.some(p => p.id === item.id)
            )]
            : response.items
        );
        setHasMore(prev => ({ ...prev, albums: !!response.cursor }));
        setCursor(prev => ({ ...prev, albums: response.cursor }));
      }
    } catch {
      showToast(`Failed to load ${currentTab}`, 'error');
      setHasMore(prev => ({ ...prev, [currentTab]: false }));
    } finally {
      setIsLoading(prev => ({ ...prev, [currentTab]: false }));
    }
  }, [user, isAuthenticated, activeTab, cursor, hasMore]);

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

    return () => {
      if (observer.current) {
        observer.current.disconnect();
      }
    };
  }, [activeTab, hasMore, isLoading]);

  useEffect(() => {
    setSavedAlbums([]);
    setSavedSongs([]);
    setCursor({});
    setHasMore({ songs: true, albums: true });
    fetchUserLibrary();
  }, [activeTab]);

  const handleSongRemoved = (songId: string) => {
    setSavedSongs(prev => prev.filter(song => song.id !== songId));
  };

  const handleAlbumRemoved = (albumId: string) => {
    setSavedAlbums(prev => prev.filter(album => album.id !== albumId));
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

        <div className="bg-white shadow rounded-lg overflow-hidden p-6">
          <div className="border-b mb-6">
            <div className="flex space-x-6">
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
          </div>

          <div>
            {activeTab === 'songs' && (
                <>
                  {savedSongs.length > 0 ? (
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
                  ) : (
                      <div className="text-center py-10">
                        <h3 className="text-lg font-medium text-gray-900 mb-1">
                          {isLoading.songs ? 'Loading...' : 'No saved songs yet'}
                        </h3>
                        <p className="text-gray-500">
                          {!isLoading.songs && 'Add songs to your library by clicking the + button next to a song'}
                        </p>
                      </div>
                  )}
                </>
            )}

            {activeTab === 'albums' && (
                <>
                  {savedAlbums.length > 0 ? (
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
                        {isLoading.albums && (
                            <div className="col-span-full flex justify-center mt-4">
                              <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-indigo-500" />
                            </div>
                        )}
                      </div>
                  ) : (
                      <div className="text-center py-10">
                        <h3 className="text-lg font-medium text-gray-900 mb-1">
                          {isLoading.albums ? 'Loading...' : 'No saved albums yet'}
                        </h3>
                        <p className="text-gray-500">
                          {!isLoading.albums && 'Add albums to your library by clicking the + button on an album'}
                        </p>
                      </div>
                  )}
                </>
            )}
          </div>
        </div>
      </div>
  );
};

export default LibraryPage;