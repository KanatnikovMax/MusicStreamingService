import React from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { 
  Music, Users, Disc, ListMusic, 
  LogOut, Home
} from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';

const Sidebar: React.FC = () => {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="hidden lg:flex flex-col bg-gradient-to-b from-indigo-600 to-indigo-800 w-64 py-6 px-4 text-white">
      <div className="flex items-center justify-center mb-8">
        <Music className="h-8 w-8 mr-2" />
        <span className="text-xl font-bold">Music Admin</span>
      </div>

      <nav className="flex-1">
        <ul>
          <li className="mb-2">
            <NavLink 
              to="/dashboard" 
              end
              className={({ isActive }) => 
                `flex items-center py-3 px-4 rounded-lg transition-colors ${
                  isActive 
                    ? 'bg-indigo-700 text-white' 
                    : 'text-indigo-100 hover:bg-indigo-700'
                }`
              }
            >
              <Home className="h-5 w-5 mr-3" />
              <span>Dashboard</span>
            </NavLink>
          </li>

          <li className="mb-2">
            <NavLink 
              to="/dashboard/artists" 
              className={({ isActive }) => 
                `flex items-center py-3 px-4 rounded-lg transition-colors ${
                  isActive 
                    ? 'bg-indigo-700 text-white' 
                    : 'text-indigo-100 hover:bg-indigo-700'
                }`
              }
            >
              <Users className="h-5 w-5 mr-3" />
              <span>Artists</span>
            </NavLink>
          </li>

          <li className="mb-2">
            <NavLink 
              to="/dashboard/albums" 
              className={({ isActive }) => 
                `flex items-center py-3 px-4 rounded-lg transition-colors ${
                  isActive 
                    ? 'bg-indigo-700 text-white' 
                    : 'text-indigo-100 hover:bg-indigo-700'
                }`
              }
            >
              <Disc className="h-5 w-5 mr-3" />
              <span>Albums</span>
            </NavLink>
          </li>

          <li className="mb-2">
            <NavLink 
              to="/dashboard/songs" 
              className={({ isActive }) => 
                `flex items-center py-3 px-4 rounded-lg transition-colors ${
                  isActive 
                    ? 'bg-indigo-700 text-white' 
                    : 'text-indigo-100 hover:bg-indigo-700'
                }`
              }
            >
              <ListMusic className="h-5 w-5 mr-3" />
              <span>Songs</span>
            </NavLink>
          </li>
        </ul>
      </nav>

      <div className="mt-auto border-t border-indigo-500 pt-4">
        <button 
          onClick={handleLogout}
          className="flex w-full items-center py-3 px-4 rounded-lg text-indigo-100 hover:bg-indigo-700 transition-colors"
        >
          <LogOut className="h-5 w-5 mr-3" />
          <span>Logout</span>
        </button>
      </div>
    </div>
  );
};

export default Sidebar;