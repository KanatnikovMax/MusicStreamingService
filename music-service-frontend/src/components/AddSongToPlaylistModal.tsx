import React from 'react';
import { Link } from 'react-router-dom';
import { ListMusic, X } from 'lucide-react';
import type { Playlist } from '../types/music';

interface AddSongToPlaylistModalProps {
  isOpen: boolean;
  playlists: Playlist[];
  isLoading: boolean;
  songTitle: string;
  onClose: () => void;
  onSelectPlaylist: (playlistId: string) => void;
}

const AddSongToPlaylistModal: React.FC<AddSongToPlaylistModalProps> = ({
  isOpen,
  playlists,
  isLoading,
  songTitle,
  onClose,
  onSelectPlaylist
}) => {
  if (!isOpen) {
    return null;
  }

  return (
      <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 px-4">
        <div className="w-full max-w-md rounded-xl bg-white shadow-xl">
          <div className="flex items-start justify-between border-b border-gray-200 p-5">
            <div>
              <h3 className="text-lg font-semibold text-gray-900">Add to Playlist</h3>
              <p className="mt-1 text-sm text-gray-500">{songTitle}</p>
            </div>
            <button
                onClick={onClose}
                className="rounded-full p-2 text-gray-500 transition-colors hover:bg-gray-100 hover:text-gray-700"
            >
              <X size={18} />
            </button>
          </div>

          <div className="max-h-80 overflow-y-auto p-4">
            {isLoading ? (
                <div className="flex justify-center py-8">
                  <div className="h-8 w-8 animate-spin rounded-full border-b-2 border-t-2 border-indigo-500" />
                </div>
            ) : playlists.length === 0 ? (
                <div className="rounded-lg border border-dashed border-gray-300 bg-gray-50 p-6 text-center">
                  <ListMusic size={32} className="mx-auto mb-3 text-gray-400" />
                  <p className="text-sm font-medium text-gray-900">No playlists yet</p>
                  <p className="mt-1 text-sm text-gray-500">Create one on the playlists page first.</p>
                  <Link
                      to="/playlists"
                      onClick={onClose}
                      className="mt-4 inline-flex rounded-md bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700"
                  >
                    Open My Playlists
                  </Link>
                </div>
            ) : (
                <div className="space-y-2">
                  {playlists.map(playlist => (
                      <button
                          key={playlist.id}
                          onClick={() => onSelectPlaylist(playlist.id)}
                          className="flex w-full items-center justify-between rounded-lg border border-gray-200 px-4 py-3 text-left transition-colors hover:border-indigo-300 hover:bg-indigo-50"
                      >
                        <div>
                          <p className="font-medium text-gray-900">{playlist.name}</p>
                          <p className="text-sm text-gray-500">Add song to this playlist</p>
                        </div>
                        <ListMusic size={18} className="text-indigo-600" />
                      </button>
                  ))}
                </div>
            )}
          </div>
        </div>
      </div>
  );
};

export default AddSongToPlaylistModal;
