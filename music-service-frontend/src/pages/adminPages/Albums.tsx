import React, { useState, useEffect, useCallback, useRef } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Edit, Trash, Search, Disc } from 'lucide-react';
import { getAllAlbums, deleteAlbum } from '../../services/albumService.ts';
import { useToast } from '../../contexts/ToastContext.tsx';
import type { Album } from "../../types/music.ts";

const Albums: React.FC = () => {
  const [albums, setAlbums] = useState<Album[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isInitialized, setIsInitialized] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [cursor, setCursor] = useState<Date>();
  const [hasMore, setHasMore] = useState(true);
  const [pageSize, setPageSize] = useState(10);
  const [deleteConfirmation, setDeleteConfirmation] = useState<{
    show: boolean;
    albumToDelete: string | null;
  }>({ show: false, albumToDelete: null });
  const { showToast } = useToast();
  const observer = useRef<IntersectionObserver | null>(null);
  const lastAlbumRef = useRef<HTMLTableRowElement>(null);
  const abortControllerRef = useRef<AbortController | null>(null);

  const calculatePageSize = useCallback(() => {
    const screenHeight = window.innerHeight;
    const rowHeight = 64;
    return Math.min(Math.max(Math.floor(screenHeight / rowHeight), 10, 30));
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

  const handleDelete = async (id: string) => {
    try {
      await deleteAlbum(id);
      setAlbums(prev => prev.filter(album => album.id !== id));
      showToast('Album deleted successfully', 'success');
    } catch {
      showToast('Failed to delete album', 'error');
    } finally {
      setDeleteConfirmation({ show: false, albumToDelete: null });
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  return (
      <div>
        {deleteConfirmation.show && (
            <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
              <div className="bg-white rounded-lg p-6 max-w-md w-full">
                <h3 className="text-lg font-semibold mb-4">Confirm Delete</h3>
                <p className="text-gray-600 mb-6">
                  Are you sure you want to delete this album? This action cannot be undone.
                </p>
                <div className="flex justify-end space-x-4">
                  <button
                      onClick={() => setDeleteConfirmation({ show: false, albumToDelete: null })}
                      className="px-4 py-2 border border-gray-300 rounded-md hover:bg-gray-50 transition-colors"
                  >
                    Cancel
                  </button>
                  <button
                      onClick={() => deleteConfirmation.albumToDelete && handleDelete(deleteConfirmation.albumToDelete)}
                      className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 transition-colors"
                  >
                    Delete
                  </button>
                </div>
              </div>
            </div>
        )}

        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Albums</h1>
          <Link
              to="/dashboard/albums/create"
              className="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-md flex items-center transition-colors"
          >
            <Plus size={20} className="mr-2" />
            Add Album
          </Link>
        </div>

        <div className="bg-white shadow rounded-lg overflow-hidden">
          <div className="p-4 border-b">
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <Search size={20} className="text-gray-400" />
              </div>
              <input
                  type="text"
                  className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  placeholder="Search albums..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
              />
            </div>
          </div>

          <div className="min-h-[400px]">
            {!isInitialized || isLoading ? (
                <div className="p-8 flex justify-center">
                  <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-purple-500"></div>
                </div>
            ) : albums.length === 0 ? (
                <div className="p-8 text-center">
                  <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-purple-100 mb-4">
                    <Disc size={32} className="text-purple-600" />
                  </div>
                  <h3 className="text-lg font-medium text-gray-900 mb-1">
                    {searchTerm ? 'No albums found' : 'Start adding albums'}
                  </h3>
                  {!searchTerm && (
                      <Link
                          to="/dashboard/albums/create"
                          className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-purple-600 hover:bg-purple-700"
                      >
                        <Plus size={16} className="mr-2" />
                        Add First Album
                      </Link>
                  )}
                </div>
            ) : (
                <div className="overflow-x-auto">
                  <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                    <tr>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Title
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Artists
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Release Date
                      </th>
                      <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Actions
                      </th>
                    </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                    {albums.map((album, index) => (
                        <tr
                            key={album.id}
                            ref={index === albums.length - 1 ? lastAlbumRef : null}
                            className="hover:bg-gray-50"
                        >
                          <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                            {album.title}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                            {album.artists.map(artist => artist.name).join(', ')}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                            {formatDate(album.releaseDate)}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                            <Link
                                to={`/dashboard/albums/edit/${album.id}`}
                                className="text-purple-600 hover:text-purple-900 mr-4"
                            >
                              <Edit size={18} className="inline" />
                            </Link>
                            <button
                                onClick={() => setDeleteConfirmation({ show: true, albumToDelete: album.id })}
                                className="text-red-600 hover:text-red-900"
                            >
                              <Trash size={18} className="inline" />
                            </button>
                          </td>
                        </tr>
                    ))}
                    </tbody>
                  </table>
                  {isLoading && (
                      <div className="p-4 flex justify-center">
                        <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-purple-500"></div>
                      </div>
                  )}
                </div>
            )}
          </div>
        </div>
      </div>
  );
};

export default Albums;