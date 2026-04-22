import React, { useCallback, useEffect, useRef, useState } from 'react';
import { Plus, Search } from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';
import { useToast } from '../../contexts/ToastContext';
import PlaylistCard from '../../components/PlaylistCard';
import { createPlaylist, getUserPlaylists } from '../../services/playlistService';
import type { Playlist } from '../../types/music';

const PlaylistsPage: React.FC = () => {
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [playlistName, setPlaylistName] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [hasMore, setHasMore] = useState(true);
  const [isLoading, setIsLoading] = useState(true);
  const [isInitialized, setIsInitialized] = useState(false);
  const [isCreating, setIsCreating] = useState(false);
  const { user, isAuthenticated } = useAuth();
  const { showToast } = useToast();
  const observer = useRef<IntersectionObserver | null>(null);
  const lastPlaylistRef = useRef<HTMLDivElement>(null);
  const cursorRef = useRef<Date>();

  const fetchPlaylists = useCallback(async (loadMore: boolean) => {
    if (!isAuthenticated || !user) {
      return;
    }

    try {
      setIsLoading(true);
      const response = await getUserPlaylists(user.id, {
        cursor: loadMore ? cursorRef.current : undefined,
        pageSize: 12,
        searchTerm
      });

      setPlaylists(prev => {
        if (!loadMore) {
          return response.items;
        }

        const existingIds = new Set(prev.map(item => item.id));
        const nextItems = response.items.filter(item => !existingIds.has(item.id));
        return [...prev, ...nextItems];
      });

      cursorRef.current = response.cursor || undefined;
      setHasMore(!!response.cursor);
    } catch {
      setHasMore(false);
      showToast('Failed to load playlists', 'error');
    } finally {
      setIsLoading(false);
      setIsInitialized(true);
    }
  }, [isAuthenticated, searchTerm, showToast, user]);

  useEffect(() => {
    const handler = setTimeout(() => {
      setPlaylists([]);
      cursorRef.current = undefined;
      setHasMore(true);
      fetchPlaylists(false);
    }, 300);

    return () => clearTimeout(handler);
  }, [fetchPlaylists, searchTerm]);

  useEffect(() => {
    if (isLoading || !hasMore) {
      return;
    }

    const callback = (entries: IntersectionObserverEntry[]) => {
      if (entries[0]?.isIntersecting && hasMore && !isLoading) {
        fetchPlaylists(true);
      }
    };

    observer.current = new IntersectionObserver(callback, {
      root: null,
      rootMargin: '100px',
      threshold: 0.1
    });

    if (lastPlaylistRef.current) {
      observer.current.observe(lastPlaylistRef.current);
    }

    return () => observer.current?.disconnect();
  }, [fetchPlaylists, hasMore, isLoading, playlists]);

  const handleCreatePlaylist = async (event: React.FormEvent) => {
    event.preventDefault();

    if (!user) {
      return;
    }

    const trimmedName = playlistName.trim();
    if (!trimmedName) {
      showToast('Playlist name is required', 'info');
      return;
    }

    try {
      setIsCreating(true);
      const playlist = await createPlaylist(user.id, { name: trimmedName });
      setPlaylists(prev => [playlist, ...prev.filter(item => item.id !== playlist.id)]);
      setPlaylistName('');
      showToast('Playlist created', 'success');
    } catch {
      showToast('Failed to create playlist', 'error');
    } finally {
      setIsCreating(false);
    }
  };

  const handlePlaylistDeleted = (playlistId: string) => {
    setPlaylists(prev => prev.filter(item => item.id !== playlistId));
  };

  if (!isAuthenticated) {
    return (
        <div className="flex h-64 flex-col items-center justify-center">
          <h2 className="mb-2 text-xl font-semibold text-gray-900">Please login to view your playlists</h2>
          <p className="text-gray-500">Your personal playlists will appear here</p>
        </div>
    );
  }

  return (
      <div>
        <div className="mb-6 flex items-center justify-between">
          <h1 className="text-2xl font-bold text-gray-900">My Playlists</h1>
        </div>

        <div className="mb-8 overflow-hidden rounded-lg bg-white shadow">
          <div className="border-b border-gray-200 p-6">
            <h2 className="text-lg font-semibold text-gray-900">Create Playlist</h2>
            <p className="mt-1 text-sm text-gray-500">Add a personal playlist and start collecting songs.</p>
            <form onSubmit={handleCreatePlaylist} className="mt-4 flex flex-col gap-3 sm:flex-row">
              <input
                  type="text"
                  value={playlistName}
                  onChange={event => setPlaylistName(event.target.value)}
                  maxLength={200}
                  placeholder="Playlist name"
                  className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm placeholder-gray-500 focus:border-indigo-500 focus:outline-none focus:ring-indigo-500"
              />
              <button
                  type="submit"
                  disabled={isCreating}
                  className="inline-flex items-center justify-center rounded-md bg-indigo-600 px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-indigo-700 disabled:cursor-not-allowed disabled:opacity-70"
              >
                <Plus size={16} className="mr-2" />
                {isCreating ? 'Creating...' : 'Create Playlist'}
              </button>
            </form>
          </div>

          <div className="p-4">
            <div className="relative">
              <div className="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
                <Search size={20} className="text-gray-400" />
              </div>
              <input
                  type="text"
                  className="block w-full rounded-md border border-gray-300 bg-white py-2 pl-10 pr-3 text-sm leading-5 placeholder-gray-500 focus:border-indigo-500 focus:outline-none focus:ring-indigo-500"
                  placeholder="Search playlists..."
                  value={searchTerm}
                  onChange={event => setSearchTerm(event.target.value)}
              />
            </div>
          </div>

          <div className="min-h-[400px]">
            {!isInitialized || isLoading ? (
                <div className="flex justify-center p-8">
                  <div className="h-12 w-12 animate-spin rounded-full border-b-2 border-t-2 border-indigo-500" />
                </div>
            ) : playlists.length === 0 ? (
                <div className="p-8 text-center">
                  <h3 className="mb-1 text-lg font-medium text-gray-900">
                    {searchTerm ? 'No playlists found' : 'No playlists yet'}
                  </h3>
                  <p className="text-gray-500">
                    {searchTerm ? 'Try a different search query' : 'Create your first playlist above.'}
                  </p>
                </div>
            ) : (
                <div className="p-6">
                  <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4">
                    {playlists.map((playlist, index) => (
                        <div
                            key={playlist.id}
                            ref={index === playlists.length - 1 ? lastPlaylistRef : null}
                        >
                          <PlaylistCard
                              playlist={playlist}
                              onPlaylistDeleted={handlePlaylistDeleted}
                          />
                        </div>
                    ))}
                  </div>

                  {isLoading && (
                      <div className="mt-8 flex justify-center">
                        <div className="h-8 w-8 animate-spin rounded-full border-b-2 border-t-2 border-indigo-500" />
                      </div>
                  )}
                </div>
            )}
          </div>
        </div>
      </div>
  );
};

export default PlaylistsPage;
