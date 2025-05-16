import React, { useState, useEffect, useCallback, useRef } from 'react';
import { Search } from 'lucide-react';
import AlbumCard from '../../components/AlbumCard.tsx';
import { getAllAlbums } from '../../services/albumService.ts';
import { getUserAlbums } from '../../services/userService.ts';
import { useToast } from '../../contexts/ToastContext.tsx';
import { useAuth } from '../../contexts/AuthContext.tsx';
import type { Album } from '../../types/music.ts';

const AlbumsPage: React.FC = () => {
  const [albums, setAlbums] = useState<Album[]>([]);
  const [userSavedAlbums, setUserSavedAlbums] = useState<string[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isInitialized, setIsInitialized] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [cursor, setCursor] = useState<Date>();
  const [hasMore, setHasMore] = useState(true);
  const [pageSize, setPageSize] = useState(16);
  const { user, isAuthenticated } = useAuth();
  const { showToast } = useToast();
  const observer = useRef<IntersectionObserver | null>(null);
  const lastAlbumRef = useRef<HTMLDivElement>(null);
  const abortControllerRef = useRef<AbortController | null>(null);

  const calculatePageSize = useCallback(() => {
    const screenWidth = window.innerWidth;
    const cardWidth = 300;
    const margin = 20;
    const columns = Math.max(1, Math.floor((screenWidth - margin) / (cardWidth + margin)));
    return columns * 3;
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

  const fetchAlbums = useCallback(async (loadMore: boolean) => {
    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
    }
    const controller = new AbortController();
    abortControllerRef.current = controller;

    try {
      setIsLoading(true);
      const response = await getAllAlbums({
        cursor: loadMore ? cursor : undefined,
        pageSize,
        searchTerm
      });

      setAlbums(prev => {
        if (!loadMore) return response.items;
        const existingIds = new Set(prev.map(item => item.id));
        const newItems = response.items.filter(item => !existingIds.has(item.id));
        return [...prev, ...newItems];
      });

      setHasMore(!!response.cursor);
      setCursor(response.cursor || undefined);
    } catch {
      setHasMore(false);
    } finally {
      if (!controller.signal.aborted) {
        setIsLoading(false);
        setIsInitialized(true);
      }
    }
  }, [cursor, pageSize, searchTerm, showToast]);

  useEffect(() => {
    const handler = setTimeout(() => {
      setAlbums([]);
      setCursor(undefined);
      setHasMore(true);
      fetchAlbums(false);
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
        fetchAlbums(true);
      }
    };

    observer.current = new IntersectionObserver(callback, {
      root: null,
      rootMargin: '100px',
      threshold: 0.1
    });

    if (lastAlbumRef.current) {
      observer.current.observe(lastAlbumRef.current);
    }

    return () => observer.current?.disconnect();
  }, [isLoading, hasMore, albums, fetchAlbums]);

  const fetchUserAlbums = useCallback(async () => {
    if (!isAuthenticated || !user) return;
    try {
      const response = await getUserAlbums(user.id, { pageSize: 100 });
      setUserSavedAlbums(response.items.map(album => album.id));
    } catch {
      showToast('Failed to load user albums', 'error');
    }
  }, [isAuthenticated, user, showToast]);

  useEffect(() => {
    fetchUserAlbums();
  }, [fetchUserAlbums]);

  const handleAlbumSaved = (albumId: string) => {
    setUserSavedAlbums(prev => [...prev, albumId]);
  };

  const handleAlbumRemoved = (albumId: string) => {
    setUserSavedAlbums(prev => prev.filter(id => id !== albumId));
  };

  return (
      <div>
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Albums</h1>
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
                  placeholder="Search albums..."
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
            ) : albums.length === 0 ? (
                <div className="p-8 text-center">
                  <h3 className="text-lg font-medium text-gray-900 mb-1">
                    {searchTerm ? 'No albums found' : 'Start exploring albums'}
                  </h3>
                  <p className="text-gray-500 mb-4">
                    {searchTerm ? 'Try different search terms' : 'Search for your favorite albums'}
                  </p>
                </div>
            ) : (
                <div className="p-6">
                  <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
                    {albums.map((album, index) => (
                        <div
                            key={album.id}
                            ref={index === albums.length - 1 ? lastAlbumRef : null}
                        >
                          <AlbumCard
                              album={album}
                              isSaved={userSavedAlbums.includes(album.id)}
                              onAlbumSaved={handleAlbumSaved}
                              onAlbumRemoved={handleAlbumRemoved}
                          />
                        </div>
                    ))}
                  </div>

                  {isLoading && (
                      <div className="mt-8 flex justify-center">
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

export default AlbumsPage;