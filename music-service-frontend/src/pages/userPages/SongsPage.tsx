import React, { useState, useEffect, useCallback, useRef } from 'react';
import { Search } from 'lucide-react';
import TrackList from '../../components/TrackList.tsx';
import { getAllSongs } from '../../services/songService.ts';
import { getUserSongs } from '../../services/userService.ts';
import { useToast } from '../../contexts/ToastContext.tsx';
import { useAuth } from '../../contexts/AuthContext.tsx';
import type { Song } from '../../types/music.ts';

const SongsPage: React.FC = () => {
  const [songs, setSongs] = useState<Song[]>([]);
  const [userSavedSongs, setUserSavedSongs] = useState<string[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isInitialized, setIsInitialized] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [cursor, setCursor] = useState<Date>();
  const [hasMore, setHasMore] = useState(true);
  const [pageSize, setPageSize] = useState(20);
  const { user, isAuthenticated } = useAuth();
  const { showToast } = useToast();
  const observer = useRef<IntersectionObserver | null>(null);
  const lastSongRef = useRef<HTMLDivElement>(null);
  const abortControllerRef = useRef<AbortController | null>(null);

  const calculatePageSize = useCallback(() => {
    const screenHeight = window.innerHeight;
    const rowHeight = 64;
    const calculatedSize = Math.floor((screenHeight - 200) / rowHeight) * 2;
    return Math.min(Math.max(calculatedSize, 20), 50);
  }, []);

  useEffect(() => {
    const handleResize = () => {
      const newSize = calculatePageSize();
      setPageSize(prev => prev !== newSize ? newSize : prev);
    };

    handleResize();
    const resizeTimer = setTimeout(handleResize, 200);
    window.addEventListener('resize', handleResize);
    return () => {
      window.removeEventListener('resize', handleResize);
      clearTimeout(resizeTimer);
    };
  }, [calculatePageSize]);

  const fetchSongs = useCallback(async (loadMore: boolean) => {
    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
    }
    const controller = new AbortController();
    abortControllerRef.current = controller;

    try {
      setIsLoading(true);
      const response = await getAllSongs({
        cursor: loadMore ? cursor : undefined,
        pageSize,
        searchTerm
      });

      setSongs(prev => {
        if (!loadMore) return response.items;

        const existingIds = new Set(prev.map(item => item.id));
        const newItems = response.items.filter(item => !existingIds.has(item.id));
        return [...prev, ...newItems];
      });

      setHasMore(!!response.cursor);
      setCursor(response.cursor ? new Date(response.cursor) : undefined);
    } catch {

      setHasMore(false);
    } finally {
      if (!controller.signal.aborted) {
        setIsLoading(false);
        setIsInitialized(true);
      }
    }
  }, [cursor, pageSize, searchTerm, showToast]);

  const fetchUserSongs = useCallback(async () => {
    if (!isAuthenticated || !user) return;

    try {
      const response = await getUserSongs(user.id, { pageSize: 100 });
      setUserSavedSongs(response.items.map(song => song.id));
    } catch {
      console.error('Failed to load user songs');
    }
  }, [isAuthenticated, user]);

  useEffect(() => {
    const handler = setTimeout(() => {
      setSongs([]);
      setCursor(undefined);
      setHasMore(true);
      fetchSongs(false);
    }, 300);

    return () => {
      clearTimeout(handler);
      if (abortControllerRef.current) {
        abortControllerRef.current.abort();
      }
    };
  }, [searchTerm, pageSize]);

  useEffect(() => {
    if (isLoading || !hasMore) return;

    const callback = (entries: IntersectionObserverEntry[]) => {
      if (entries[0]?.isIntersecting && hasMore && !isLoading) {
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
  }, [isLoading, hasMore, songs, fetchSongs]);

  useEffect(() => {
    fetchUserSongs();
  }, [fetchUserSongs]);

  const handleSongSaved = (songId: string) => {
    setUserSavedSongs(prev => [...prev, songId]);
  };

  const handleSongRemoved = (songId: string) => {
    setUserSavedSongs(prev => prev.filter(id => id !== songId));
  };

  return (
      <div>
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Discover Music</h1>
        </div>

        <div className="bg-white shadow rounded-lg overflow-hidden mb-8">
          <div className="p-4 border-b">
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <Search size={20} className="text-gray-400" />
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

          <div className="min-h-[400px]">
            {!isInitialized || isLoading ? (
                <div className="p-8 flex justify-center">
                  <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500" />
                </div>
            ) : songs.length === 0 ? (
                <div className="p-8 text-center">
                  <h3 className="text-lg font-medium text-gray-900 mb-1">
                    {searchTerm ? 'No songs found' : 'Start exploring music'}
                  </h3>
                  {!searchTerm && (
                      <p className="text-gray-500 mb-4">
                        Try searching for your favorite songs or artists
                      </p>
                  )}
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

                  {isLoading && (
                      <div className="p-4 flex justify-center">
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

export default SongsPage;