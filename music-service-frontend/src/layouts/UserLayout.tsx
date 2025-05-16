import React from 'react';
import { Outlet } from 'react-router-dom';
import UserHeader from '../components/UserHeader';
import UserSidebar from '../components/UserSidebar';
import MusicPlayer from '../components/MusicPlayer';
import { useMusicPlayer } from '../contexts/MusicPlayerContext';

const UserLayout: React.FC = () => {
    const { currentSong } = useMusicPlayer();

    return (
        <div className="flex flex-col h-screen">
            <UserHeader />
            <div className="flex flex-1 overflow-hidden mt-16">
                <UserSidebar />
                <main className="flex-1 overflow-x-hidden overflow-y-auto bg-gray-100 p-6 pb-24">
                    <Outlet />
                </main>
            </div>
            {currentSong && <MusicPlayer />}
        </div>
    );
};

export default UserLayout;