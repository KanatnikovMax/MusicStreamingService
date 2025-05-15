import React from 'react';
import { Users, Disc, Music, Info, Edit2, LucideDownload } from 'lucide-react';
import { Link } from 'react-router-dom';

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

const TipCard: React.FC<{
    title: string;
    description: string;
    icon: React.ReactNode;
    color: string;
}> = ({ title, description, icon, color }) => (
    <div className="bg-white rounded-lg shadow-md p-6 flex items-start">
        <div className={`flex items-center justify-center w-12 h-12 rounded-full ${color} text-white mr-4`}>
            {icon}
        </div>
        <div>
            <h3 className="text-lg font-semibold text-gray-900 mb-2">{title}</h3>
            <p className="text-sm text-gray-600">{description}</p>
        </div>
    </div>
);

const Overview: React.FC = () => {
    const tips = [
        {
            title: "Regular Backups",
            description: "Export your library data periodically to prevent data loss",
            icon: <LucideDownload size={20} />,
            color: "bg-blue-500"
        },
        {
            title: "Proper Metadata Formatting",
            description: "Fill in all metadata fields for optimal display in the application",
            icon: <Info size={20} />,
            color: "bg-green-500"
        },
        {
            title: "Content Editing",
            description: "You can modify song, album and artist information at any time",
            icon: <Edit2 size={20} />,
            color: "bg-yellow-500"
        }
    ];

    return (
        <div className="p-4">
            <div className="mb-8">
                <h1 className="text-2xl font-bold text-gray-900 mb-2">Welcome to Music Admin</h1>
                <p className="text-gray-600">
                    Your music library management dashboard
                </p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
                {tips.map((tip, index) => (
                    <TipCard
                        key={index}
                        title={tip.title}
                        description={tip.description}
                        icon={tip.icon}
                        color={tip.color}
                    />
                ))}
            </div>

            <h2 className="text-xl font-semibold text-gray-900 mb-4">Quick Actions</h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <ActionCard
                    title="Add Artist"
                    description="Create a new artist profile"
                    icon={<Users size={20} />}
                    link="/dashboard/artists/create"
                    color="bg-indigo-600"
                />
                <ActionCard
                    title="Add Album"
                    description="Create a new album with tracks"
                    icon={<Disc size={20} />}
                    link="/dashboard/albums/create"
                    color="bg-purple-600"
                />
                <ActionCard
                    title="Upload Track"
                    description="Add a new song to the library"
                    icon={<Music size={20} />}
                    link="/dashboard/songs/create"
                    color="bg-pink-600"
                />
            </div>
        </div>
    );
};

export default Overview;