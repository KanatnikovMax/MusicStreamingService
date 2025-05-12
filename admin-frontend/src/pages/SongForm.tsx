import React, { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Save, X, Upload } from 'lucide-react';
import { createSong, getSongById, updateSong } from '../services/songService';
import { getAllAlbums } from '../services/albumService';
import { getAllArtists } from '../services/artistService';
import { useToast } from '../contexts/ToastContext';
import type { PaginationRequest } from '../types/pagination';

interface ArtistOption {
  value: string;
  label: string;
}

interface AlbumOption {
  value: string;
  label: string;
}

const SongForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showToast } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [showAlbumDropdown, setShowAlbumDropdown] = useState(false);

  const [title, setTitle] = useState('');
  const [trackNumber, setTrackNumber] = useState('1');
  const [albumId, setAlbumId] = useState('');
  const [albumInput, setAlbumInput] = useState('');
  const [selectedArtists, setSelectedArtists] = useState<string[]>([]);
  const [audioFile, setAudioFile] = useState<File | null>(null);
  const [duration, setDuration] = useState(0);

  const [albumOptions, setAlbumOptions] = useState<AlbumOption[]>([]);
  const [artistOptions, setArtistOptions] = useState<ArtistOption[]>([]);
  const [artistInput, setArtistInput] = useState('');
  const [searchTimeout, setSearchTimeout] = useState<{
    album: ReturnType<typeof setTimeout> | null;
    artist: ReturnType<typeof setTimeout> | null;
  }>({ album: null, artist: null });
  const [cursor, setCursor] = useState<{
    album: Date | undefined;
    artist: Date | undefined;
  }>({ album: undefined, artist: undefined });
  const [pageSize] = useState(20);

  const isEditMode = !!id;

  useEffect(() => {
    const abortController = new AbortController();

    const fetchData = async () => {
      if (isEditMode) {
        await fetchSong();
      } else {
        await Promise.all([fetchAlbums(''), fetchArtists('')]);
      }
    };

    fetchData();

    return () => abortController.abort();
  }, []);

  const fetchAlbums = useCallback(async (searchTerm: string, loadMore = false) => {
    try {
      const request: PaginationRequest<Date> & { searchTerm?: string } = {
        cursor: loadMore ? cursor.album : undefined,
        pageSize,
        searchTerm: searchTerm || undefined
      };

      const response = await getAllAlbums(request);

      setAlbumOptions(prev =>
          loadMore
              ? [...prev, ...response.items.map(album => ({
                value: album.id,
                label: album.title
              }))]
              : response.items.map(album => ({
                value: album.id,
                label: album.title
              }))
      );

      setCursor(prev => ({
        ...prev,
        album: response.cursor ? new Date(response.cursor) : undefined
      }));
    } catch {
      if (!isEditMode) showToast('Failed to load albums', 'error');
    }
  }, [cursor.album, pageSize, showToast, isEditMode]);

  const fetchArtists = useCallback(async (searchTerm: string, loadMore = false) => {
    try {
      const request: PaginationRequest<Date> & { searchTerm?: string } = {
        cursor: loadMore ? cursor.artist : undefined,
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

      setCursor(prev => ({
        ...prev,
        artist: response.cursor ? new Date(response.cursor) : undefined
      }));
    } catch {
      if (!isEditMode) showToast('Failed to load artists', 'error');
    }
  }, [cursor.artist, pageSize, showToast, isEditMode]);

  const handleAlbumInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setAlbumInput(value);
    setShowAlbumDropdown(true);

    if (searchTimeout.album) clearTimeout(searchTimeout.album);
    setSearchTimeout(prev => ({
      ...prev,
      album: setTimeout(() => {
        if (value.trim()) {
          fetchAlbums(value.trim());
        } else {
          setAlbumOptions([]);
        }
      }, 300)
    }));
  };

  const handleAlbumBlur = () => {
    setTimeout(() => {
      setShowAlbumDropdown(false);
    }, 200);
  };

  const handleArtistInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setArtistInput(value);

    if (searchTimeout.artist) clearTimeout(searchTimeout.artist);
    setSearchTimeout(prev => ({
      ...prev,
      artist: setTimeout(() => {
        if (value.trim()) {
          fetchArtists(value.trim());
        } else {
          setArtistOptions([]);
        }
      }, 300)
    }));
  };

  const fetchSong = async () => {
    try {
      setIsLoading(true);
      const songData = await getSongById(id!);
      setTitle(songData.title);
      setTrackNumber(songData.trackNumber.toString());
      setDuration(songData.duration);
    } catch {
      showToast('Failed to load song data', 'error');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const trackNumberValue = parseInt(trackNumber);
    if (isNaN(trackNumberValue)) {
      showToast('Please enter a valid track number', 'error');
      return;
    }

    if (!title.trim() || !trackNumberValue || trackNumberValue < 1) {
      showToast('Please fill in all required fields correctly', 'error');
      return;
    }

    try {
      setIsSaving(true);

      const formData = new FormData();
      formData.append('Title', title);
      formData.append('TrackNumber', trackNumberValue.toString());

      if (!isEditMode) {
        if (!albumId || selectedArtists.length === 0 || !audioFile) {
          showToast('Please fill in all required fields', 'error');
          return;
        }
        formData.append('AlbumId', albumId);
        selectedArtists.forEach(artist => formData.append('Artists', artist));
        formData.append('AudioFile', audioFile);
        formData.append('Duration', duration.toString());
      }

      if (isEditMode) {
        await updateSong(id!, formData);
        showToast('Song updated successfully', 'success');
      } else {
        await createSong(formData);
        showToast('Song created successfully', 'success');
      }

      navigate('/dashboard/songs');
    } catch {
      showToast(`Failed to ${isEditMode ? 'update' : 'create'} song`, 'error');
    } finally {
      setIsSaving(false);
    }
  };

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files && files.length > 0) {
      const file = files[0];

      if (file.type !== 'audio/mpeg') {
        showToast('Only MP3 files are allowed', 'error');
        return;
      }

      if (file.size > 30 * 1024 * 1024) {
        showToast('File size exceeds 30MB limit', 'error');
        return;
      }

      const objectUrl = URL.createObjectURL(file);
      const audio = new Audio(objectUrl);

      audio.onloadedmetadata = () => {
        setDuration(Math.floor(audio.duration));
        URL.revokeObjectURL(objectUrl);
      };

      audio.onerror = () => {
        showToast('Could not read audio file', 'error');
        URL.revokeObjectURL(objectUrl);
      };

      setAudioFile(file);
    }
  };

  const handleAddArtist = (artistName: string) => {
    if (artistName.trim() && !selectedArtists.includes(artistName)) {
      setSelectedArtists([...selectedArtists, artistName]);
      setArtistInput('');
    }
  };

  const handleSelectAlbum = (albumId: string, albumTitle: string) => {
    setAlbumId(albumId);
    setAlbumInput(albumTitle);
    setShowAlbumDropdown(false);
  };

  const handleRemoveArtist = (artistName: string) => {
    setSelectedArtists(selectedArtists.filter(name => name !== artistName));
  };

  const filteredAlbums = albumOptions
      .filter(option => option.label.toLowerCase().includes(albumInput.toLowerCase()));

  const filteredArtists = artistOptions
      .filter(option => !selectedArtists.includes(option.value))
      .filter(option => option.label.toLowerCase().includes(artistInput.toLowerCase()));

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
              onClick={() => navigate('/dashboard/songs')}
              className="mr-4 p-2 rounded-full hover:bg-gray-200 transition-colors"
          >
            <ArrowLeft size={20} />
          </button>
          <h1 className="text-2xl font-bold text-gray-900">
            {isEditMode ? 'Edit Song' : 'Upload New Song'}
          </h1>
        </div>

        <div className="bg-white shadow rounded-lg overflow-hidden">
          <form onSubmit={handleSubmit} className="p-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
              <div>
                <label htmlFor="title" className="block text-sm font-medium text-gray-700 mb-1">
                  Title <span className="text-red-500">*</span>
                </label>
                <input
                    type="text"
                    id="title"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                    placeholder="Enter song title"
                    required
                />
              </div>

              <div>
                <label htmlFor="trackNumber" className="block text-sm font-medium text-gray-700 mb-1">
                  Track Number <span className="text-red-500">*</span>
                </label>
                <input
                    type="number"
                    id="trackNumber"
                    value={trackNumber}
                    onChange={(e) => setTrackNumber(e.target.value)}
                    min="1"
                    className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                    required
                />
              </div>
            </div>

            {!isEditMode && (
                <>
                  <div className="mb-6">
                    <label htmlFor="album" className="block text-sm font-medium text-gray-700 mb-1">
                      Album <span className="text-red-500">*</span>
                    </label>
                    <div className="relative">
                      <input
                          type="text"
                          id="album"
                          value={albumInput}
                          onChange={handleAlbumInputChange}
                          onFocus={() => setShowAlbumDropdown(true)}
                          onBlur={handleAlbumBlur}
                          className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                          placeholder="Search album..."
                      />
                      {showAlbumDropdown && filteredAlbums.length > 0 && (
                          <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg max-h-40 overflow-y-auto">
                            {filteredAlbums.map((album) => (
                                <div
                                    key={album.value}
                                    className="px-3 py-2 hover:bg-gray-100 cursor-pointer"
                                    onClick={() => handleSelectAlbum(album.value, album.label)}
                                >
                                  {album.label}
                                </div>
                            ))}
                          </div>
                      )}
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
                              const match = filteredArtists.find(opt =>
                                  opt.label.toLowerCase() === artistInput.toLowerCase()
                              );
                              if (match) handleAddArtist(match.value);
                            }
                          }}
                          className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                          placeholder="Type artist name"
                      />
                      {artistInput && filteredArtists.length > 0 && (
                          <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg max-h-40 overflow-y-auto">
                            {filteredArtists.map((option) => (
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

                  <div className="mb-6">
                    <label htmlFor="audioFile" className="block text-sm font-medium text-gray-700 mb-1">
                      MP3 File <span className="text-red-500">*</span>
                    </label>
                    <div className="mt-1 flex justify-center px-6 pt-5 pb-6 border-2 border-gray-300 border-dashed rounded-md">
                      <div className="space-y-1 text-center">
                        <Upload className="mx-auto h-12 w-12 text-gray-400" />
                        <div className="flex text-sm text-gray-600">
                          <label
                              htmlFor="audioFile"
                              className="relative cursor-pointer bg-white rounded-md font-medium text-purple-600 hover:text-purple-500 focus-within:outline-none focus-within:ring-2 focus-within:ring-offset-2 focus-within:ring-purple-500"
                          >
                            <span>Upload a file</span>
                            <input
                                id="audioFile"
                                name="audioFile"
                                type="file"
                                className="sr-only"
                                accept="audio/mpeg"
                                onChange={handleFileChange}
                                required
                            />
                          </label>
                          <p className="pl-1">or drag and drop</p>
                        </div>
                        <p className="text-xs text-gray-500">MP3 up to 30MB</p>
                        {audioFile && (
                            <p className="text-sm text-green-600">
                              Selected: {audioFile.name}
                              {duration > 0 && ` (${Math.floor(duration / 60)}:${(duration % 60).toString().padStart(2, '0')})`}
                            </p>
                        )}
                      </div>
                    </div>
                  </div>
                </>
            )}

            <div className="flex justify-end">
              <button
                  type="button"
                  onClick={() => navigate('/dashboard/songs')}
                  className="mr-4 bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
              >
                Cancel
              </button>
              <button
                  type="submit"
                  disabled={isSaving || (!isEditMode && (!audioFile || selectedArtists.length === 0 || !albumId))}
                  className={`bg-purple-600 py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 flex items-center ${
                      isSaving ? 'opacity-70 cursor-not-allowed' : ''
                  }`}
              >
                <Save size={18} className="mr-2" />
                {isSaving ? 'Saving...' : 'Save Song'}
              </button>
            </div>
          </form>
        </div>
      </div>
  );
};

export default SongForm;