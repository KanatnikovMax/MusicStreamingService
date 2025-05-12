import React, { useState, useEffect, useCallback, useRef } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Edit, Trash, Search, Music } from 'lucide-react';
import { getAllSongs, deleteSong } from '../services/songService';
import { useToast } from '../contexts/ToastContext';

export interface Song {
  id: string;
  title: string;
  duration: number;
  trackNumber: number;
  albumId: string;
  artists: { id: string; name: string }[];
}

const Songs: React.FC = () => {
  const [songs, setSongs] = useState<Song[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [cursor, setCursor] = useState<Date>();
  const [hasMore, setHasMore] = useState(true);
  const [pageSize, setPageSize] = useState(10);
  const [deleteConfirmation, setDeleteConfirmation] = useState<{
    show: boolean;
    songToDelete: string | null;
  }>({ show: false, songToDelete: null });
  const { showToast } = useToast();
  const loaderRef = useRef<HTMLDivElement>(null);

  const calculatePageSize = useCallback(() => {
    const screenHeight = window.innerHeight;
    const screenWidth = window.innerWidth;

    let baseSize = 10;

    if (screenWidth >= 1200) {
      baseSize = Math.floor(screenHeight / 80);
    } else if (screenWidth >= 768) {
      baseSize = Math.floor(screenHeight / 100);
    } else {
      baseSize = Math.floor(screenHeight / 120);
    }

    return Math.min(Math.max(baseSize, 5), 20);
  }, []);

  useEffect(() => {
    const handleResize = () => {
      const newSize = calculatePageSize();
      if (newSize !== pageSize) {
        setPageSize(newSize);
      }
    };

    handleResize();
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, [calculatePageSize, pageSize]);

  const fetchSongs = useCallback(async (loadMore = false) => {
    if (isLoading || !hasMore) return;

    try {
      setIsLoading(true);
      const response = await getAllSongs({
        cursor: loadMore ? cursor : undefined,
        pageSize: pageSize,
        searchTerm: searchTerm || undefined
      });

      setSongs(prev =>
          loadMore
              ? [...prev, ...response.items.filter(newItem => !prev.some(item => item.id === newItem.id))]
              : response.items
      );
      setCursor(response.cursor ?? undefined);
      setHasMore(response.items.length >= pageSize);
    } catch (error) {
      console.error('Error:', error);
      showToast('Failed to load songs', 'error');
      setHasMore(false);
    } finally {
      setIsLoading(false);
    }
  }, [cursor, isLoading, hasMore, pageSize, searchTerm, showToast]);

  useEffect(() => {
    const handler = setTimeout(() => {
      setSongs([]);
      setCursor(undefined);
      setHasMore(true);
      fetchSongs();
    }, 500);

    return () => clearTimeout(handler);
  }, [searchTerm, pageSize]);

  const handleScroll = useCallback(() => {
    if (!loaderRef.current || isLoading || !hasMore || searchTerm) return;

    const { top } = loaderRef.current.getBoundingClientRect();
    if (top < window.innerHeight + 500) {
      fetchSongs(true);
    }
  }, [isLoading, hasMore, searchTerm, fetchSongs]);

  useEffect(() => {
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, [handleScroll]);

  const handleDelete = async (id: string) => {
    try {
      await deleteSong(id);
      setSongs(prev => prev.filter(song => song.id !== id));
      showToast('Song deleted successfully', 'success');
    } catch {
      showToast('Failed to delete song', 'error');
    } finally {
      setDeleteConfirmation({ show: false, songToDelete: null });
    }
  };

  const formatDuration = (seconds: number) => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
  };

  return (
      <div>
        {/* Delete Confirmation Modal */}
        {deleteConfirmation.show && (
            <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
              <div className="bg-white rounded-lg p-6 max-w-md w-full">
                <h3 className="text-lg font-semibold mb-4">Confirm Delete</h3>
                <p className="text-gray-600 mb-6">
                  Are you sure you want to delete this song? This action cannot be undone.
                </p>
                <div className="flex justify-end space-x-4">
                  <button
                      onClick={() => setDeleteConfirmation({ show: false, songToDelete: null })}
                      className="px-4 py-2 border border-gray-300 rounded-md hover:bg-gray-50 transition-colors"
                  >
                    Cancel
                  </button>
                  <button
                      onClick={() => deleteConfirmation.songToDelete && handleDelete(deleteConfirmation.songToDelete)}
                      className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 transition-colors"
                  >
                    Delete
                  </button>
                </div>
              </div>
            </div>
        )}

        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Songs</h1>
          <Link
              to="/dashboard/songs/create"
              className="bg-pink-600 hover:bg-pink-700 text-white px-4 py-2 rounded-md flex items-center transition-colors"
          >
            <Plus size={20} className="mr-2" />
            Add Song
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
                  className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:ring-pink-500 focus:border-pink-500 sm:text-sm"
                  placeholder="Search songs..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
              />
            </div>
          </div>

          {isLoading && songs.length === 0 ? (
              <div className="p-8 flex justify-center">
                <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-pink-500"></div>
              </div>
          ) : songs.length === 0 ? (
              <div className="p-8 text-center">
                <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-pink-100 mb-4">
                  <Music size={32} className="text-pink-600" />
                </div>
                <h3 className="text-lg font-medium text-gray-900 mb-1">No songs found</h3>
                <p className="text-gray-500 mb-4">
                  {searchTerm ? 'No songs match your search criteria' : 'Start by adding your first song'}
                </p>
                {!searchTerm && (
                    <Link
                        to="/dashboard/songs/create"
                        className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-pink-600 hover:bg-pink-700"
                    >
                      <Plus size={16} className="mr-2" />
                      Add Song
                    </Link>
                )}
              </div>
          ) : (
              <>
                <div className="overflow-x-auto">
                  <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                    <tr>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        #
                      </th>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Title
                      </th>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Artists
                      </th>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Duration
                      </th>
                      <th scope="col" className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Actions
                      </th>
                    </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                    {songs.map((song) => (
                        <tr key={song.id} className="hover:bg-gray-50">
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                            {song.trackNumber}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                            {song.title}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                            {song.artists.map(artist => artist.name).join(', ')}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                            {formatDuration(song.duration)}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                            <Link
                                to={`/dashboard/songs/edit/${song.id}`}
                                className="text-pink-600 hover:text-pink-900 mr-4"
                            >
                              <Edit size={18} className="inline" />
                            </Link>
                            <button
                                onClick={() => setDeleteConfirmation({ show: true, songToDelete: song.id })}
                                className="text-red-600 hover:text-red-900"
                            >
                              <Trash size={18} className="inline" />
                            </button>
                          </td>
                        </tr>
                    ))}
                    </tbody>
                  </table>
                </div>

                {hasMore && !searchTerm && (
                    <div ref={loaderRef} className="h-2 flex justify-center p-4">
                      {isLoading && (
                          <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-pink-500"></div>
                      )}
                    </div>
                )}
              </>
          )}
        </div>
      </div>
  );
};

export default Songs;