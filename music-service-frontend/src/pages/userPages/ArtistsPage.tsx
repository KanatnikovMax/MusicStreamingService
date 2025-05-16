import React, { useState, useEffect, useCallback, useRef } from 'react';
import { Search } from 'lucide-react';
import ArtistCard from '../../components/ArtistCard.tsx';
import { getAllArtists } from '../../services/artistService.ts';
import { useToast } from '../../contexts/ToastContext.tsx';
import type { Artist } from '../../types/music.ts';

const ArtistsPage: React.FC = () => {
  const [artists, setArtists] = useState<Artist[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isInitialized, setIsInitialized] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [cursor, setCursor] = useState<Date>();
  const [hasMore, setHasMore] = useState(true);
  const [pageSize, setPageSize] = useState(16);
  const { showToast } = useToast();
  const observer = useRef<IntersectionObserver | null>(null);
  const lastArtistRef = useRef<HTMLDivElement>(null);
  const abortControllerRef = useRef<AbortController | null>(null);

  const calculatePageSize = useCallback(() => {
    const screenWidth = window.innerWidth;
    const screenHeight = window.innerHeight;
    let columns = 1;
    if (screenWidth >= 1280) columns = 4;
    else if (screenWidth >= 1024) columns = 3;
    else if (screenWidth >= 768) columns = 2;
    const cardHeight = 320;
    const rows = Math.floor((screenHeight - 200) / cardHeight);
    return Math.min(Math.max(columns * rows, 4), 24);
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

  const fetchArtists = useCallback(async (loadMore: boolean) => {
    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
    }
    const controller = new AbortController();
    abortControllerRef.current = controller;

    try {
      setIsLoading(true);
      const response = await getAllArtists({
        cursor: loadMore ? cursor : undefined,
        pageSize,
        searchTerm
      });

      setArtists(prev => {
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
      setArtists([]);
      setCursor(undefined);
      setHasMore(true);
      fetchArtists(false);
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
        fetchArtists(true);
      }
    };

    observer.current = new IntersectionObserver(callback, {
      root: null,
      rootMargin: '100px',
      threshold: 0.1
    });

    if (lastArtistRef.current) {
      observer.current.observe(lastArtistRef.current);
    }

    return () => observer.current?.disconnect();
  }, [isLoading, hasMore, artists, fetchArtists]);

  return (
      <div>
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Artists</h1>
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
                  placeholder="Search artists..."
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
            ) : artists.length === 0 ? (
                <div className="p-8 text-center">
                  <h3 className="text-lg font-medium text-gray-900 mb-1">
                    {searchTerm ? 'No artists found' : 'Start exploring artists'}
                  </h3>
                  <p className="text-gray-500 mb-4">
                    {searchTerm ? 'Try different search terms' : 'Search for your favorite artists'}
                  </p>
                </div>
            ) : (
                <div className="p-6">
                  <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
                    {artists.map((artist, index) => (
                        <div
                            key={artist.id}
                            ref={index === artists.length - 1 ? lastArtistRef : null}
                        >
                          <ArtistCard artist={artist} />
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

export default ArtistsPage;