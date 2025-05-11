import React, {useState, useEffect, useCallback} from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Save, X } from 'lucide-react';
import { createAlbum, getAlbumById, updateAlbum } from '../services/albumService';
import { getAllArtists } from '../services/artistService';
import { useToast } from '../contexts/ToastContext';

interface Artist {
  id: string;
  name: string;
}

interface ArtistOption {
  value: string;
  label: string;
}

interface PaginationRequest<T> {
  cursor?: T;
  pageSize: number;
  searchTerm?: string;
}

const AlbumForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showToast } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);

  const [title, setTitle] = useState('');
  const [releaseDate, setReleaseDate] = useState('');
  const [selectedArtists, setSelectedArtists] = useState<string[]>([]);
  const [artistOptions, setArtistOptions] = useState<ArtistOption[]>([]);
  const [artistInput, setArtistInput] = useState('');
  const [searchTimeout, setSearchTimeout] = useState<ReturnType<typeof setTimeout> | null>(null);
  const [cursor, setCursor] = useState<Date>();
  const [pageSize] = useState(20);

  const isEditMode = !!id;

  useEffect(() => {
    fetchArtists('');
    if (isEditMode) {
      fetchAlbum();
    }
  }, [id]);

  const fetchArtists = useCallback(async (searchTerm: string, loadMore = false) => {
    try {
      const request: PaginationRequest<Date> = {
        cursor: loadMore ? cursor : undefined,
        pageSize,
        searchTerm: searchTerm || undefined
      };

      const response = await getAllArtists(request);

      setArtistOptions(prev =>
          loadMore
              ? [...prev, ...response.items.map(artist => ({
                value: artist.name,
                label: artist.name
              }))]
              : response.items.map(artist => ({
                value: artist.name,
                label: artist.name
              }))
      );

      setCursor(response.cursor ? new Date(response.cursor) : undefined);
    } catch {
      showToast('Failed to load artists', 'error');
    }
  }, [cursor, pageSize, showToast]);

  const handleArtistInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setArtistInput(value);

    if (searchTimeout) clearTimeout(searchTimeout);
    setSearchTimeout(
        setTimeout(() => {
          if (value.trim()) {
            fetchArtists(value.trim());
          } else {
            setArtistOptions([]);
          }
        }, 300)
    );
  };

  const fetchAlbum = async () => {
    try {
      setIsLoading(true);
      const albumData = await getAlbumById(id!);
      setTitle(albumData.title);
      setReleaseDate(new Date(albumData.releaseDate).toISOString().split('T')[0]);
      setSelectedArtists(albumData.artists.map((artist: Artist) => artist.name));
    } catch {
      showToast('Failed to load album data', 'error');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!title.trim() || !releaseDate || selectedArtists.length === 0) {
      showToast('Please fill in all required fields', 'error');
      return;
    }

    try {
      setIsSaving(true);

      const albumData = {
        title,
        releaseDate: new Date(releaseDate).toISOString(),
        artists: selectedArtists
      };

      if (isEditMode) {
        await updateAlbum(id!, albumData);
        showToast('Album updated successfully', 'success');
      } else {
        await createAlbum(albumData);
        showToast('Album created successfully', 'success');
      }

      navigate('/dashboard/albums');
    } catch {
      showToast(`Failed to ${isEditMode ? 'update' : 'create'} album`, 'error');
    } finally {
      setIsSaving(false);
    }
  };

  const handleAddArtist = (artistName: string) => {
    if (artistName.trim() && !selectedArtists.includes(artistName)) {
      setSelectedArtists([...selectedArtists, artistName]);
      setArtistInput('');
    }
  };

  const handleRemoveArtist = (artistName: string) => {
    setSelectedArtists(selectedArtists.filter(name => name !== artistName));
  };

  const filteredOptions = artistOptions
      .filter(option => !selectedArtists.includes(option.value))
      .filter(option =>
          option.label.toLowerCase().includes(artistInput.toLowerCase())
      );

  if (isLoading) {
    return (
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-purple-500"></div>
        </div>
    );
  }

  return (
      <div>
        <div className="flex items-center mb-6">
          <button
              onClick={() => navigate('/dashboard/albums')}
              className="mr-4 p-2 rounded-full hover:bg-gray-200 transition-colors"
          >
            <ArrowLeft size={20} />
          </button>
          <h1 className="text-2xl font-bold text-gray-900">
            {isEditMode ? 'Edit Album' : 'Create New Album'}
          </h1>
        </div>

        <div className="bg-white shadow rounded-lg overflow-hidden">
          <form onSubmit={handleSubmit} className="p-6">
            <div className="mb-6">
              <label htmlFor="title" className="block text-sm font-medium text-gray-700 mb-1">
                Album Title <span className="text-red-500">*</span>
              </label>
              <input
                  type="text"
                  id="title"
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
                  className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  placeholder="Enter album title"
                  required
              />
            </div>

            <div className="mb-6">
              <label htmlFor="releaseDate" className="block text-sm font-medium text-gray-700 mb-1">
                Release Date <span className="text-red-500">*</span>
              </label>
              <input
                  type="date"
                  id="releaseDate"
                  value={releaseDate}
                  onChange={(e) => setReleaseDate(e.target.value)}
                  className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  required
              />
            </div>

            <div className="mb-6">
              <label htmlFor="artists" className="block text-sm font-medium text-gray-700 mb-1">
                Artists <span className="text-red-500">*</span>
              </label>
              <div className="relative">
                <input
                    type="text"
                    id="artists"
                    value={artistInput}
                    onChange={handleArtistInputChange}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') {
                        e.preventDefault();
                        const match = filteredOptions.find(opt =>
                            opt.label.toLowerCase() === artistInput.toLowerCase()
                        );
                        if (match) handleAddArtist(match.value);
                      }
                    }}
                    className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                    placeholder="Type artist name"
                />
                {filteredOptions.length > 0 && artistInput && (
                    <div
                        className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg max-h-40 overflow-y-auto">
                      {filteredOptions.map((option) => (
                          <div
                              key={option.value}
                              className="px-3 py-2 hover:bg-gray-100 cursor-pointer"
                              onClick={() => handleAddArtist(option.value)}
                          >
                            {option.label}
                          </div>
                      ))}
                    </div>
                )}
              </div>

              {selectedArtists.length > 0 && (
                  <div className="mt-2 flex flex-wrap gap-2">
                    {selectedArtists.map((artistName) => (
                        <div
                            key={artistName}
                            className="bg-purple-100 text-purple-800 px-3 py-1 rounded-full text-sm flex items-center"
                        >
                          {artistName}
                          <button
                              type="button"
                              onClick={() => handleRemoveArtist(artistName)}
                              className="ml-1 text-purple-600 hover:text-purple-800"
                          >
                            <X size={16} />
                          </button>
                        </div>
                    ))}
                  </div>
              )}
              {selectedArtists.length === 0 && (
                  <p className="mt-2 text-sm text-gray-500">Please add at least one artist</p>
              )}
            </div>

            <div className="flex justify-end">
              <button
                  type="button"
                  onClick={() => navigate('/dashboard/albums')}
                  className="mr-4 bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
              >
                Cancel
              </button>
              <button
                  type="submit"
                  disabled={isSaving || selectedArtists.length === 0}
                  className={`bg-purple-600 py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 flex items-center ${
                      (isSaving || selectedArtists.length === 0) ? 'opacity-70 cursor-not-allowed' : ''
                  }`}
              >
                <Save size={18} className="mr-2" />
                {isSaving ? 'Saving...' : 'Save Album'}
              </button>
            </div>
          </form>
        </div>
      </div>
  );
};

export default AlbumForm;