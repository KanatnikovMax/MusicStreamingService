import React, { useEffect, useState } from 'react';
import TrackList from '../../components/TrackList.tsx';
import { getListeningHistory } from '../../services/userService.ts';
import { useAuth } from '../../contexts/AuthContext.tsx';
import { useToast } from '../../contexts/ToastContext.tsx';
import type { Song } from '../../types/music.ts';

const ListeningHistoryPage: React.FC = () => {
  const [history, setHistory] = useState<Song[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const { user, isAuthenticated } = useAuth();
  const { showToast } = useToast();

  useEffect(() => {
    const fetchListeningHistory = async () => {
      if (!isAuthenticated || !user) {
        setIsLoading(false);
        return;
      }

      try {
        const items = await getListeningHistory(user.id);
        setHistory(items);
      } catch {
        showToast('Failed to load listening history', 'error');
      } finally {
        setIsLoading(false);
      }
    };

    fetchListeningHistory();
  }, [isAuthenticated, user, showToast]);

  if (!isAuthenticated) {
    return (
      <div className="flex flex-col items-center justify-center h-64">
        <h2 className="text-xl font-semibold text-gray-900 mb-2">Please login to view listening history</h2>
        <p className="text-gray-500">Recently played songs will appear here</p>
      </div>
    );
  }

  return (
    <div>
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Listening History</h1>
      </div>

      <div className="bg-white shadow rounded-lg overflow-hidden min-h-[400px]">
        {isLoading ? (
          <div className="p-8 flex justify-center">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500" />
          </div>
        ) : history.length === 0 ? (
          <div className="text-center py-10">
            <h3 className="text-lg font-medium text-gray-900 mb-1">No listening history yet</h3>
            <p className="text-gray-500">Start listening to songs and they will appear here</p>
          </div>
        ) : (
          <div className="divide-y divide-gray-200">
            <TrackList
              songs={history}
              showLibraryActions={false}
            />
          </div>
        )}
      </div>
    </div>
  );
};

export default ListeningHistoryPage;
