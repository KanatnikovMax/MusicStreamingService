import React from 'react';
import { Link } from 'react-router-dom';
import { Music } from 'lucide-react';
import type {Artist} from '../types/music';

interface ArtistCardProps {
  artist: Artist;
}

const ArtistCard: React.FC<ArtistCardProps> = ({ artist }) => {
  return (
      <Link
          to={`/artists/${artist.id}`}
          className="block bg-white rounded-lg shadow-md overflow-hidden transition-transform hover:shadow-lg hover:-translate-y-1"
      >
        <div className="aspect-square bg-gray-100 flex items-center justify-center overflow-hidden">
          {artist.photoBase64 ? (
              <img
                  src={`data:image/jpeg;base64,${artist.photoBase64}`}
                  alt={artist.name}
                  className="w-full h-full object-cover"
              />
          ) : (
              <div className="h-24 w-24 rounded-full bg-purple-600 flex items-center justify-center">
                <Music size={48} className="text-white" />
              </div>
          )}
        </div>

        <div className="p-4">
          <h3 className="text-lg font-semibold text-gray-900 hover:text-indigo-600 text-center truncate">
            {artist.name}
          </h3>
          {artist.albums && (
              <div className="mt-1 text-sm text-gray-500 text-center">
                {artist.albums.length} {artist.albums.length === 1 ? 'album' : 'albums'}
              </div>
          )}
        </div>
      </Link>
  );
};

export default ArtistCard;