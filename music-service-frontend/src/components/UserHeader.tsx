import React, { useState } from 'react';
import { Menu, X, Music, Users, Disc, Home, LogOut, BookMarked, Settings, LayoutDashboard, LogIn } from 'lucide-react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const UserHeader: React.FC = () => {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const { user, logout, isAuthenticated, isAdmin } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
      <header className="bg-white shadow-sm z-40 fixed top-0 left-0 right-0">
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
            <span className="text-lg font-semibold text-gray-800">Music Stream</span>
          </div>

          <div className="hidden lg:block">
            <h1 className="text-xl font-semibold text-gray-800">Music Streaming Service</h1>
          </div>

          <div className="hidden lg:flex items-center gap-4">
            {isAuthenticated ? (
                <>
                  {!isAdmin && (
                      <NavLink
                          to="/settings"
                          className="text-gray-600 hover:text-indigo-600"
                      >
                        <Settings size={20} />
                      </NavLink>
                  )}
                  <button
                      onClick={handleLogout}
                      className="text-gray-600 hover:text-indigo-600"
                  >
                    <LogOut size={20} />
                  </button>
                  <span className="text-gray-600">
                {user?.username}
              </span>
                </>
            ) : (
                <NavLink
                    to="/login"
                    className="text-gray-600 hover:text-indigo-600"
                >
                  <LogIn size={20} />
                </NavLink>
            )}
          </div>
        </div>

        {isMobileMenuOpen && (
            <div className="lg:hidden bg-indigo-600 text-white z-50 fixed top-16 left-0 right-0 h-[calc(100vh-4rem)] overflow-y-auto">
              <nav className="px-4 py-2">
                <ul className="space-y-2">
                  {isAuthenticated ? (
                      <>
                        {!isAdmin && (
                            <li>
                              <NavLink
                                  to="/settings"
                                  onClick={() => setIsMobileMenuOpen(false)}
                                  className={({ isActive }) =>
                                      `flex items-center py-3 px-4 rounded-lg transition-colors ${
                                          isActive
                                              ? 'bg-indigo-700 text-white'
                                              : 'text-indigo-100 hover:bg-indigo-700'
                                      }`
                                  }
                              >
                                <Settings className="h-5 w-5 mr-3" />
                                <span>Settings</span>
                              </NavLink>
                            </li>
                        )}
                        <li>
                          <button
                              onClick={() => {
                                setIsMobileMenuOpen(false);
                                handleLogout();
                              }}
                              className="w-full flex items-center py-3 px-4 rounded-lg text-indigo-100 hover:bg-indigo-700 transition-colors"
                          >
                            <LogOut className="h-5 w-5 mr-3" />
                            <span>Logout</span>
                          </button>
                        </li>
                      </>
                  ) : (
                      <li>
                        <NavLink
                            to="/login"
                            onClick={() => setIsMobileMenuOpen(false)}
                            className="flex items-center py-3 px-4 rounded-lg text-indigo-100 hover:bg-indigo-700 transition-colors"
                        >
                          <LogIn className="h-5 w-5 mr-3" />
                          <span>Login</span>
                        </NavLink>
                      </li>
                  )}

                  <li>
                    <NavLink
                        to="/"
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
                      <span>Home</span>
                    </NavLink>
                  </li>

                  <li>
                    <NavLink
                        to="/artists"
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
                        to="/albums"
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

                  {isAuthenticated && !isAdmin && (
                      <li>
                        <NavLink
                            to="/library"
                            onClick={() => setIsMobileMenuOpen(false)}
                            className={({ isActive }) =>
                                `flex items-center py-3 px-4 rounded-lg transition-colors ${
                                    isActive
                                        ? 'bg-indigo-700 text-white'
                                        : 'text-indigo-100 hover:bg-indigo-700'
                                }`
                            }
                        >
                          <BookMarked className="h-5 w-5 mr-3" />
                          <span>My Library</span>
                        </NavLink>
                      </li>
                  )}

                  {isAdmin && (
                      <li>
                        <NavLink
                            to="/dashboard"
                            onClick={() => setIsMobileMenuOpen(false)}
                            className={({ isActive }) =>
                                `flex items-center py-3 px-4 rounded-lg transition-colors ${
                                    isActive
                                        ? 'bg-indigo-700 text-white'
                                        : 'text-indigo-100 hover:bg-indigo-700'
                                }`
                            }
                        >
                          <LayoutDashboard className="h-5 w-5 mr-3" />
                          <span>Dashboard</span>
                        </NavLink>
                      </li>
                  )}
                </ul>
              </nav>
            </div>
        )}
      </header>
  );
};

export default UserHeader;