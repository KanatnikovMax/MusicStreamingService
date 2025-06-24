import React, { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Save, Image, Trash } from 'lucide-react';
import { createArtist, getArtistById, updateArtist } from '../../services/artistService.ts';
import { useToast } from '../../contexts/ToastContext.tsx';

const ArtistForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showToast } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [name, setName] = useState('');
  const [photo, setPhoto] = useState<File | undefined>(undefined);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const isEditMode = !!id;

  useEffect(() => {
    if (isEditMode) {
      fetchArtist();
    }
  }, [id]);

  const fetchArtist = async () => {
    try {
      setIsLoading(true);
      const artistData = await getArtistById(id!);
      const artist = artistData.artists[0];
      setName(artist.name);

      if (artist.photoBase64) {
        setPreviewUrl(`data:image/jpeg;base64,${artist.photoBase64}`);
      }
    } catch {
      showToast('Failed to load artist data', 'error');
    } finally {
      setIsLoading(false);
    }
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

    if (!name.trim()) {
      showToast('Please enter artist name', 'error');
      return;
    }

    try {
      setIsSaving(true);

      if (isEditMode) {
        await updateArtist(id!, {
          name,
          photo: photo || undefined // Явное преобразование null в undefined
        });
        showToast('Artist updated successfully', 'success');
      } else {
        await createArtist({
          name,
          photo: photo || undefined // Явное преобразование null в undefined
        });
        showToast('Artist created successfully', 'success');
      }

      navigate('/dashboard/artists');
    } catch {
      showToast(`Failed to ${isEditMode ? 'update' : 'create'} artist`, 'error');
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return (
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
        </div>
    );
  }

  return (
      <div>
        <div className="flex items-center mb-6">
          <button
              onClick={() => navigate('/dashboard/artists')}
              className="mr-4 p-2 rounded-full hover:bg-gray-200 transition-colors"
          >
            <ArrowLeft size={20} />
          </button>
          <h1 className="text-2xl font-bold text-gray-900">
            {isEditMode ? 'Edit Artist' : 'Create New Artist'}
          </h1>
        </div>

        <div className="bg-white shadow rounded-lg overflow-hidden">
          <form onSubmit={handleSubmit} className="p-6">
            <div className="mb-6">
              <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-1">
                Artist Name
              </label>
              <input
                  type="text"
                  id="name"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  placeholder="Enter artist name"
                  required
              />
            </div>

            <div className="mb-6">
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Artist Photo
              </label>

              <div className="flex items-start space-x-4">
                <div className="flex-shrink-0">
                  {previewUrl ? (
                      <div className="relative group">
                        <img
                            src={previewUrl}
                            alt="Preview"
                            className="h-24 w-24 rounded-full object-cover border-2 border-gray-300"
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
                      <div className="h-24 w-24 rounded-full bg-gray-200 border-2 border-dashed border-gray-400 flex items-center justify-center">
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
                    file:bg-indigo-50 file:text-indigo-700
                    hover:file:bg-indigo-100"
                  />
                  <p className="mt-1 text-xs text-gray-500">
                    PNG, JPG, JPEG up to 5MB
                  </p>
                </div>
              </div>
            </div>

            <div className="flex justify-end">
              <button
                  type="button"
                  onClick={() => navigate('/dashboard/artists')}
                  className="mr-4 bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              >
                Cancel
              </button>
              <button
                  type="submit"
                  disabled={isSaving}
                  className="bg-indigo-600 py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 flex items-center"
              >
                <Save size={18} className="mr-2" />
                {isSaving ? 'Saving...' : 'Save Artist'}
              </button>
            </div>
          </form>
        </div>
      </div>
  );
};

export default ArtistForm;