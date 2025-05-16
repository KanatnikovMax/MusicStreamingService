import React, { createContext, useState, useContext, useEffect, useCallback } from 'react';
import type {Song} from '../types/music';
import { getAlbumSongs } from '../services/albumService';

interface MusicPlayerContextType {
  currentSong: Song | null;
  isPlaying: boolean;
  queue: Song[];
  setCurrentSong: (song: Song) => void;
  togglePlay: () => void;
  nextSong: () => void;
  previousSong: () => void;
  addToQueue: (song: Song) => void;
  clearQueue: () => void;
  loadSongs: (albumId: string) => Promise<Song[]>;
  closePlayer: () => void;
}

const MusicPlayerContext = createContext<MusicPlayerContextType | null>(null);

export const useMusicPlayer = () => {
  const context = useContext(MusicPlayerContext);
  if (!context) {
    throw new Error('useMusicPlayer must be used within a MusicPlayerProvider');
  }
  return context;
};

export const MusicPlayerProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [currentSong, setCurrentSong] = useState<Song | null>(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [queue, setQueue] = useState<Song[]>([]);
  const [history, setHistory] = useState<Song[]>([]);

  useEffect(() => {
    if (currentSong) {
      setIsPlaying(true);
    }
  }, [currentSong]);

  const togglePlay = useCallback(() => {
    setIsPlaying(prev => !prev);
  }, []);

  const nextSong = useCallback(() => {
    if (queue.length === 0) return;

    if (currentSong) {
      setHistory(prev => [...prev, currentSong]);
    }

    const nextSong = queue[0];
    const newQueue = queue.slice(1);

    setCurrentSong(nextSong);
    setQueue(newQueue);
  }, [currentSong, queue]);

  const previousSong = useCallback(() => {
    if (history.length === 0) return;

    const prevSong = history[history.length - 1];
    const newHistory = history.slice(0, -1);

    if (currentSong) {
      setQueue(prev => [currentSong, ...prev]);
    }

    setCurrentSong(prevSong);
    setHistory(newHistory);
  }, [currentSong, history]);

  const addToQueue = useCallback((song: Song) => {
    setQueue(prev => [...prev, song]);
  }, []);

  const clearQueue = useCallback(() => {
    setQueue([]);
  }, []);

  const loadSongs = useCallback(async (albumId: string): Promise<Song[]> => {
    try {
      const response = await getAlbumSongs(albumId);
      return response.items;
    } catch (error) {
      console.error('Failed to load album songs:', error);
      return [];
    }
  }, []);

  const handleSetCurrentSong = useCallback((song: Song) => {
    if (currentSong) {
      setHistory(prev => [...prev, currentSong]);
    }
    setCurrentSong(song);
  }, [currentSong]);

  const closePlayer = useCallback(() => {
    setCurrentSong(null);
    setIsPlaying(false);
    setQueue([]);
    setHistory([]);
  }, []);

  return (
      <MusicPlayerContext.Provider
          value={{
            currentSong,
            isPlaying,
            queue,
            setCurrentSong: handleSetCurrentSong,
            togglePlay,
            nextSong,
            previousSong,
            addToQueue,
            clearQueue,
            loadSongs,
            closePlayer,
          }}
      >
        {children}
      </MusicPlayerContext.Provider>
  );
};