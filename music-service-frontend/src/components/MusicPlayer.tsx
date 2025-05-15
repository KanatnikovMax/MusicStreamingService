import React, { useState, useRef, useEffect } from 'react';
import { Play, Pause, SkipBack, SkipForward, Volume2, VolumeX, X } from 'lucide-react';
import { useMusicPlayer } from '../contexts/MusicPlayerContext';

const MusicPlayer: React.FC = () => {
  const {
    currentSong,
    isPlaying,
    togglePlay,
    nextSong,
    previousSong,
    closePlayer,
  } = useMusicPlayer();

  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);
  const [volume, setVolume] = useState(0.7);
  const [isMuted, setIsMuted] = useState(false);

  const audioRef = useRef<HTMLAudioElement>(null);
  const progressRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (audioRef.current) {
      if (isPlaying) {
        audioRef.current.play().catch(err => console.error('Playback error:', err));
      } else {
        audioRef.current.pause();
      }
    }
  }, [isPlaying, currentSong]);

  useEffect(() => {
    if (audioRef.current) {
      audioRef.current.volume = isMuted ? 0 : volume;
    }
  }, [volume, isMuted]);

  const handleTimeUpdate = () => {
    if (audioRef.current) {
      setCurrentTime(audioRef.current.currentTime);
      setDuration(audioRef.current.duration);
    }
  };

  const handleProgressClick = (e: React.MouseEvent<HTMLDivElement>) => {
    if (!progressRef.current || !audioRef.current) return;

    const rect = progressRef.current.getBoundingClientRect();
    const pos = (e.clientX - rect.left) / rect.width;
    audioRef.current.currentTime = pos * duration;
  };

  const formatTime = (time: number) => {
    if (isNaN(time)) return '0:00';
    const minutes = Math.floor(time / 60);
    const seconds = Math.floor(time % 60);
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
  };

  const toggleMute = () => {
    setIsMuted(!isMuted);
  };

  if (!currentSong) {
    return null;
  }

  return (
      <div
          className="fixed bottom-0 left-64 right-0 bg-gradient-to-r from-indigo-900 to-purple-900
        text-white p-3 shadow-lg z-30 transform translate-y-0 transition-transform
        w-[calc(100%-16rem)] border-t border-indigo-700"
      >
        <button
            onClick={closePlayer}
            className="absolute top-2 right-2 p-1 hover:bg-indigo-700 rounded-full transition-colors"
            aria-label="Close player"
        >
          <X size={20} />
        </button>

        <audio
            ref={audioRef}
            src={`http://localhost:5071/songs/${currentSong.id}/audio`}
            onTimeUpdate={handleTimeUpdate}
            onEnded={nextSong}
        />

        <div className="flex flex-col md:flex-row items-center gap-4">
          {/* Song Info */}
          <div className="flex items-center flex-1 min-w-0">
            <div className="w-12 h-12 bg-gray-800 rounded-md mr-3 flex-shrink-0" />
            <div className="truncate">
              <div className="font-medium truncate">{currentSong.title}</div>
              <div className="text-xs text-gray-300 truncate">
                {currentSong.artists.map(a => a.name).join(', ')}
              </div>
            </div>
          </div>

          {/* Controls */}
          <div className="flex-1 w-full max-w-2xl">
            <div className="flex items-center justify-center mb-2">
              <button
                  onClick={previousSong}
                  className="p-2 hover:bg-indigo-700 rounded-full transition-colors mr-4"
              >
                <SkipBack size={20} />
              </button>

              <button
                  onClick={togglePlay}
                  className="p-3 bg-white text-indigo-900 rounded-full hover:bg-gray-100 transition-colors mx-4"
              >
                {isPlaying ? <Pause size={24} /> : <Play size={24} />}
              </button>

              <button
                  onClick={nextSong}
                  className="p-2 hover:bg-indigo-700 rounded-full transition-colors ml-4"
              >
                <SkipForward size={20} />
              </button>
            </div>

            {/* Progress Bar */}
            <div className="flex items-center">
              <span className="text-xs mr-2">{formatTime(currentTime)}</span>
              <div
                  ref={progressRef}
                  className="h-1 flex-grow bg-gray-700 rounded-full cursor-pointer"
                  onClick={handleProgressClick}
              >
                <div
                    className="h-full bg-indigo-400 rounded-full transition-all duration-300"
                    style={{ width: `${(currentTime / duration) * 100}%` }}
                />
              </div>
              <span className="text-xs ml-2">{formatTime(duration)}</span>
            </div>
          </div>

          {/* Volume Controls */}
          <div className="flex items-center justify-end flex-1 min-w-0">
            <button
                onClick={toggleMute}
                className="p-2 hover:bg-indigo-700 rounded-full transition-colors mr-2"
            >
              {isMuted ? <VolumeX size={20} /> : <Volume2 size={20} />}
            </button>
            <input
                type="range"
                min="0"
                max="1"
                step="0.01"
                value={volume}
                onChange={(e) => setVolume(parseFloat(e.target.value))}
                className="w-24 h-1 bg-gray-700 rounded-full appearance-none cursor-pointer"
            />
          </div>
        </div>
      </div>
  );
};

export default MusicPlayer;