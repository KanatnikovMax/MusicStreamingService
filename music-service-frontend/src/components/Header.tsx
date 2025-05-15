import React, { useState } from 'react';
import { Menu, X, Music, Users, Disc, ListMusic, Home, LogOut } from 'lucide-react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const Header: React.FC = () => {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const {logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <header className="bg-white shadow-sm z-10">
      <div className="flex justify-between items-center px-6 py-4">
        <div className="flex items-center lg:hidden">
          <button
            onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
            className="text-gray-600 focus:outline-none"
          >
            {isMobileMenuOpen ? <X size={24} /> : <Menu size={24} />}
          </button>
        </div>
        
        <div className="lg:hidden flex items-center mx-auto">
          <Music className="h-6 w-6 text-indigo-600 mr-2" />
          <span className="text-lg font-semibold text-gray-800">Music Admin</span>
        </div>

        <div className="hidden lg:block">
          <h1 className="text-xl font-semibold text-gray-800">Admin Dashboard</h1>
        </div>
      </div>

      {/* Mobile Menu */}
      {isMobileMenuOpen && (
        <div className="lg:hidden bg-indigo-600 text-white">
          <nav className="px-4 py-2">
            <ul>
              <li>
                <NavLink 
                  to="/dashboard" 
                  end
                  onClick={() => setIsMobileMenuOpen(false)}
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

              <li>
                <NavLink 
                  to="/dashboard/artists" 
                  onClick={() => setIsMobileMenuOpen(false)}
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

              <li>
                <NavLink 
                  to="/dashboard/albums" 
                  onClick={() => setIsMobileMenuOpen(false)}
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

              <li>
                <NavLink 
                  to="/dashboard/songs" 
                  onClick={() => setIsMobileMenuOpen(false)}
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

              <li>
                <button 
                  onClick={() => {
                    setIsMobileMenuOpen(false);
                    handleLogout();
                  }}
                  className="flex w-full items-center py-3 px-4 rounded-lg text-indigo-100 hover:bg-indigo-700 transition-colors"
                >
                  <LogOut className="h-5 w-5 mr-3" />
                  <span>Logout</span>
                </button>
              </li>
            </ul>
          </nav>
        </div>
      )}
    </header>
  );
};

export default Header;