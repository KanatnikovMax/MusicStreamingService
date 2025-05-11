import React from 'react';
import { Users, Disc, Music, Plus } from 'lucide-react';
import { Link } from 'react-router-dom';

const StatCard: React.FC<{
  title: string;
  value: string;
  icon: React.ReactNode;
  color: string;
}> = ({ title, value, icon, color }) => (
  <div className="bg-white rounded-lg shadow-md p-6 flex items-center">
    <div className={`flex items-center justify-center w-12 h-12 rounded-full ${color} text-white mr-4`}>
      {icon}
    </div>
    <div>
      <p className="text-sm font-medium text-gray-600">{title}</p>
      <p className="text-2xl font-semibold text-gray-900">{value}</p>
    </div>
  </div>
);

const ActionCard: React.FC<{
  title: string;
  description: string;
  icon: React.ReactNode;
  link: string;
  color: string;
}> = ({ title, description, icon, link, color }) => (
  <Link to={link} className="block bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow duration-300">
    <div className="p-6">
      <div className={`flex items-center justify-center w-12 h-12 rounded-full ${color} text-white mb-4`}>
        {icon}
      </div>
      <h3 className="text-lg font-semibold text-gray-900 mb-2">{title}</h3>
      <p className="text-sm text-gray-600">{description}</p>
    </div>
  </Link>
);

const Overview: React.FC = () => {
  // In a real app, these values would come from API calls
  const stats = {
    artists: "0",
    albums: "0",
    songs: "0"
  };

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900 mb-2">Welcome to Music Admin</h1>
        <p className="text-gray-600">
          Manage your music streaming platform content from this dashboard.
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <StatCard 
          title="Total Artists" 
          value={stats.artists} 
          icon={<Users size={24} />} 
          color="bg-indigo-600" 
        />
        <StatCard 
          title="Total Albums" 
          value={stats.albums} 
          icon={<Disc size={24} />} 
          color="bg-purple-600" 
        />
        <StatCard 
          title="Total Songs" 
          value={stats.songs} 
          icon={<Music size={24} />} 
          color="bg-pink-600" 
        />
      </div>

      <h2 className="text-xl font-semibold text-gray-900 mb-4">Quick Actions</h2>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <ActionCard 
          title="Add New Artist" 
          description="Create a new artist profile in your music library." 
          icon={<Plus size={20} />} 
          link="/dashboard/artists/create" 
          color="bg-indigo-600" 
        />
        <ActionCard 
          title="Add New Album" 
          description="Add a new album with its metadata and artist associations." 
          icon={<Plus size={20} />} 
          link="/dashboard/albums/create" 
          color="bg-purple-600" 
        />
        <ActionCard 
          title="Upload New Song" 
          description="Upload MP3 files with all the metadata information." 
          icon={<Plus size={20} />} 
          link="/dashboard/songs/create" 
          color="bg-pink-600" 
        />
      </div>
    </div>
  );
};

export default Overview;