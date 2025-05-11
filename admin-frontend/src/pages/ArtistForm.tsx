import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Save } from 'lucide-react';
import { createArtist, getArtistById, updateArtist } from '../services/artistService';
import { useToast } from '../contexts/ToastContext';

const ArtistForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showToast } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [name, setName] = useState('');

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
      setName(artistData.name);
    } catch {
      showToast('Failed to load artist data', 'error');
    } finally {
      setIsLoading(false);
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
        await updateArtist(id!, { name });
        showToast('Artist updated successfully', 'success');
      } else {
        await createArtist({ name });
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