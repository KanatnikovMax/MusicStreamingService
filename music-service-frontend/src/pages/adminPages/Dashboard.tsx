import React from 'react';
import { Routes, Route } from 'react-router-dom';
import Sidebar from '../../components/Sidebar.tsx';
import Header from '../../components/Header.tsx';
import Artists from './Artists.tsx';
import Albums from './Albums.tsx';
import Songs from './Songs.tsx';
import ArtistForm from './ArtistForm.tsx';
import AlbumForm from './AlbumForm.tsx';
import SongForm from './SongForm.tsx';
import Overview from './Overview.tsx';

const Dashboard: React.FC = () => {
  return (
      <div className="flex h-screen bg-gray-100">
        <Sidebar />
        <div className="flex-1 flex flex-col overflow-hidden">
          <Header />
          <main className="flex-1 overflow-x-hidden overflow-y-auto bg-gray-100 p-6">
            <Routes>
              <Route path="/" element={<Overview />} />
              <Route path="/artists" element={<Artists />} />
              <Route path="/artists/create" element={<ArtistForm />} />
              <Route path="/artists/edit/:id" element={<ArtistForm />} />
              <Route path="/albums" element={<Albums />} />
              <Route path="/albums/create" element={<AlbumForm />} />
              <Route path="/albums/edit/:id" element={<AlbumForm />} />
              <Route path="/songs" element={<Songs />} />
              <Route path="/songs/create" element={<SongForm />} />
              <Route path="/songs/edit/:id" element={<SongForm />} />
            </Routes>
          </main>
        </div>
      </div>
  );
};

export default Dashboard;