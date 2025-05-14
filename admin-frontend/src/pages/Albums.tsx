import React, { useState, useEffect, useCallback } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Edit, Trash, Search, Disc } from 'lucide-react';
import { getAllAlbums, deleteAlbum } from '../services/albumService';
import { useToast } from '../contexts/ToastContext';

export interface Album {
  id: string;
  title: string;
  releaseDate: string;
  artists: { id: string; name: string }[];
}

const Albums: React.FC = () => {
  const [albums, setAlbums] = useState<Album[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [cursor, setCursor] = useState<Date>();
  const [hasMore, setHasMore] = useState(true);
  const [pageSize, setPageSize] = useState(10);
  const [deleteConfirmation, setDeleteConfirmation] = useState<{
    show: boolean;
    albumToDelete: string | null;
  }>({ show: false, albumToDelete: null });
  const { showToast } = useToast();

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

  const fetchAlbums = async (loadMore = false) => {
    try {
      setIsLoading(true);
      const response = await getAllAlbums({
        cursor: loadMore ? cursor : undefined,
        pageSize: pageSize,
        searchTerm: searchTerm || undefined
      });

      setAlbums(prev =>
          loadMore
              ? [...prev, ...response.items.filter(newItem => !prev.some(item => item.id === newItem.id))]
              : response.items
      );
      setCursor(response.cursor ?? undefined);
      setHasMore(response.items.length >= pageSize);
    } catch {
      showToast('Failed to load albums', 'error');
      setHasMore(false);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    const handler = setTimeout(() => {
      setAlbums([]);
      setCursor(undefined);
      setHasMore(true);
      fetchAlbums();
    }, 500);

    return () => clearTimeout(handler);
  }, [searchTerm, pageSize]);

  const handleLoadMore = () => {
    if (!isLoading && hasMore) {
      fetchAlbums(true);
    }
  };

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
        {/* Delete Confirmation Modal */}
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

          {isLoading && albums.length === 0 ? (
              <div className="p-8 flex justify-center">
                <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-purple-500"></div>
              </div>
          ) : albums.length === 0 ? (
              <div className="p-8 text-center">
                <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-purple-100 mb-4">
                  <Disc size={32} className="text-purple-600" />
                </div>
                <h3 className="text-lg font-medium text-gray-900 mb-1">No albums found</h3>
                <p className="text-gray-500 mb-4">
                  {searchTerm ? 'No albums match your search criteria' : 'Start by adding your first album'}
                </p>
                {!searchTerm && (
                    <Link
                        to="/dashboard/albums/create"
                        className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-purple-600 hover:bg-purple-700"
                    >
                      <Plus size={16} className="mr-2" />
                      Add Album
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
                        Title
                      </th>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Artists
                      </th>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Release Date
                      </th>
                      <th scope="col" className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Actions
                      </th>
                    </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                    {albums.map((album) => (
                        <tr key={album.id} className="hover:bg-gray-50">
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
                </div>

                {hasMore && !searchTerm && (
                    <div className="px-6 py-4 border-t">
                      <button
                          onClick={handleLoadMore}
                          disabled={isLoading}
                          className={`w-full py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 ${
                              isLoading ? 'opacity-70 cursor-not-allowed' : ''
                          }`}
                      >
                        {isLoading ? 'Loading...' : 'Load More'}
                      </button>
                    </div>
                )}
              </>
          )}
        </div>
      </div>
  );
};

export default Albums;