import React, {useState, useEffect, useRef, useCallback} from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Save, X, Image, Trash } from 'lucide-react';
import { createAlbum, getAlbumById, updateAlbum } from '../../services/albumService.ts';
import { getAllArtists } from '../../services/artistService.ts';
import { useToast } from '../../contexts/ToastContext.tsx';

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

  const [photo, setPhoto] = useState<File | undefined>(undefined);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const isEditMode = !!id;

  useEffect(() => {
    const abortController = new AbortController();

    const fetchData = async () => {
      if (isEditMode) {
        try {
          setIsLoading(true);
          const albumData = await getAlbumById(id!);
          const album = albumData.albums[0];
          setTitle(album.title);
          setReleaseDate(album.releaseDate);
          setSelectedArtists(album.artists.map((artist: Artist) => artist.name));

          if (album.photoBase64) {
            setPreviewUrl(`data:image/jpeg;base64,${album.photoBase64}`);
          }
        } catch {
          if (!abortController.signal.aborted) {
            showToast('Failed to load album data', 'error');
          }
        } finally {
          setIsLoading(false);
        }
      } else {
        fetchArtists('');
      }
    };

    fetchData();

    return () => abortController.abort();
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
      if (!isEditMode) showToast('Failed to load artists', 'error');
    }
  }, [cursor, pageSize, showToast, isEditMode]);

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

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      const file = e.target.files[0];

      if (!file.type.match('image.*')) {
        showToast('Please select an image file', 'error');
        return;
      }

      if (file.size > 5 * 1024 * 1024) {
        showToast('File size exceeds 5MB limit', 'error');
        return;
      }

      setPhoto(file);
      setPreviewUrl(URL.createObjectURL(file));
    }
  };

  const handleRemovePhoto = () => {
    setPhoto(undefined);
    setPreviewUrl(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
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
        artists: selectedArtists,
        photo
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
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Album Cover
              </label>

              <div className="flex items-start space-x-4">
                <div className="flex-shrink-0">
                  {previewUrl ? (
                      <div className="relative group">
                        <img
                            src={previewUrl}
                            alt="Preview"
                            className="h-24 w-24 rounded object-cover border-2 border-gray-300"
                        />
                        <button
                            type="button"
                            onClick={handleRemovePhoto}
                            className="absolute top-0 right-0 bg-red-600 rounded-full p-1 opacity-0 group-hover:opacity-100 transition-opacity"
                        >
                          <Trash size={16} className="text-white" />
                        </button>
                      </div>
                  ) : (
                      <div className="h-24 w-24 rounded bg-gray-200 border-2 border-dashed border-gray-400 flex items-center justify-center">
                        <Image size={32} className="text-gray-500" />
                      </div>
                  )}
                </div>

                <div className="flex-1">
                  <input
                      type="file"
                      ref={fileInputRef}
                      onChange={handleFileChange}
                      accept="image/*"
                      className="block w-full text-sm text-gray-500
                      file:mr-4 file:py-2 file:px-4
                      file:rounded-md file:border-0
                      file:text-sm file:font-semibold
                      file:bg-purple-50 file:text-purple-700
                      hover:file:bg-purple-100"
                  />
                  <p className="mt-1 text-xs text-gray-500">
                    PNG, JPG, JPEG up to 5MB
                  </p>
                </div>
              </div>
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
                    <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg max-h-40 overflow-y-auto">
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