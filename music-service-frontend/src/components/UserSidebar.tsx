import React from 'react';
import { NavLink } from 'react-router-dom';
import {Music, Users, Disc, BookMarked, ListMusic} from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';

const UserSidebar: React.FC = () => {
    const { isAuthenticated } = useAuth();

    return (
        <div className="hidden lg:flex flex-col bg-gradient-to-b from-indigo-600 to-indigo-800 w-64 py-4 px-4 text-white">
            <div className="flex items-center justify-center mb-4">
                <Music className="h-8 w-8 mr-2" />
                <span className="text-xl font-bold">Music Stream</span>
            </div>

            <nav>
                <ul className="space-y-1">
                    <li>
                        <NavLink
                            to="/"
                            end
                            className={({ isActive }) =>
                                `flex items-center py-2 px-4 rounded-lg transition-colors ${
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
                        <NavLink
                            to="/artists"
                            className={({ isActive }) =>
                                `flex items-center py-2 px-4 rounded-lg transition-colors ${
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
                            className={({ isActive }) =>
                                `flex items-center py-2 px-4 rounded-lg transition-colors ${
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

                    {isAuthenticated && (
                        <li>
                            <NavLink
                                to="/library"
                                className={({ isActive }) =>
                                    `flex items-center py-2 px-4 rounded-lg transition-colors ${
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
                </ul>
            </nav>
        </div>
    );
};

export default UserSidebar;