import React, { useState, useEffect, useCallback } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Edit, Trash, Search, Users } from 'lucide-react';
import { getAllArtists, deleteArtist } from '../services/artistService';
import { useToast } from '../contexts/ToastContext';

export interface Artist {
  id: string;
  name: string;
  createdAt: string;
}

const Artists: React.FC = () => {
  const [artists, setArtists] = useState<Artist[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [cursor, setCursor] = useState<Date>();
  const [hasMore, setHasMore] = useState(true);
  const [pageSize, setPageSize] = useState(10);
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

  const fetchArtists = async (loadMore = false) => {
    try {
      setIsLoading(true);
      const response = await getAllArtists({
        cursor: loadMore ? cursor : undefined,
        pageSize: pageSize,
        searchTerm: searchTerm || undefined
      });

      setArtists(prev =>
          loadMore
              ? [...prev, ...response.items.filter(newItem => !prev.some(item => item.id === newItem.id))]
              : response.items
      );
      setCursor(response.cursor ? new Date(response.cursor) : undefined);
      setHasMore(!!response.cursor);
    } catch {
      showToast('Failed to load artists', 'error');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    const handler = setTimeout(() => {
      setArtists([]);
      setCursor(undefined);
      setHasMore(true);
      fetchArtists();
    }, 500);

    return () => clearTimeout(handler);
  }, [searchTerm, pageSize]);

  const handleLoadMore = () => {
    if (!isLoading && hasMore) {
      fetchArtists(true);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Are you sure you want to delete this artist?')) {
      try {
        await deleteArtist(id);
        setArtists(prev => prev.filter(artist => artist.id !== id));
        showToast('Artist deleted successfully', 'success');
      } catch {
        showToast('Failed to delete artist', 'error');
      }
    }
  };

  return (
      <div>
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Artists</h1>
          <Link
              to="/dashboard/artists/create"
              className="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-md flex items-center transition-colors"
          >
            <Plus size={20} className="mr-2" />
            Add Artist
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
                  className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  placeholder="Search artists..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
              />
            </div>
          </div>

          {isLoading && artists.length === 0 ? (
              <div className="p-8 flex justify-center">
                <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
              </div>
          ) : artists.length === 0 ? (
              <div className="p-8 text-center">
                <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-indigo-100 mb-4">
                  <Users size={32} className="text-indigo-600" />
                </div>
                <h3 className="text-lg font-medium text-gray-900 mb-1">No artists found</h3>
                <p className="text-gray-500 mb-4">
                  {searchTerm ? 'No artists match your search criteria' : 'Start by adding your first artist'}
                </p>
                {!searchTerm && (
                    <Link
                        to="/dashboard/artists/create"
                        className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700"
                    >
                      <Plus size={16} className="mr-2" />
                      Add Artist
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
                        Name
                      </th>
                      <th scope="col" className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Actions
                      </th>
                    </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                    {artists.map((artist) => (
                        <tr key={artist.id} className="hover:bg-gray-50">
                          <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                            {artist.name}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                            <Link
                                to={`/dashboard/artists/edit/${artist.id}`}
                                className="text-indigo-600 hover:text-indigo-900 mr-4"
                            >
                              <Edit size={18} className="inline" />
                            </Link>
                            <button
                                onClick={() => handleDelete(artist.id)}
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
                          className={`w-full py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 ${
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

export default Artists;