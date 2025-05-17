import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { ToastProvider } from './contexts/ToastContext';
import { MusicPlayerProvider } from './contexts/MusicPlayerContext';
import Dashboard from './pages/adminPages/Dashboard.tsx';
import Login from './pages/auth/Login.tsx';
import Register from './pages/auth/Register.tsx';
import ProtectedRoute from './components/ProtectedRoute';
import UserLayout from './layouts/UserLayout';
import Songs from './pages/userPages/SongsPage.tsx';
import AlbumsPage from './pages/userPages/AlbumsPage.tsx';
import ArtistsPage from './pages/userPages/ArtistsPage.tsx';
import AlbumDetailPage from './pages/userPages/AlbumDetailPage.tsx';
import ArtistDetailPage from './pages/userPages/ArtistDetailPage.tsx';
import LibraryPage from './pages/userPages/LibraryPage.tsx';
import UserSettings from './pages/userPages/UserSettings.tsx';

const AppRoutes = () => {
    return (
        <Routes>
            <Route path="/" element={<UserLayout />}>
                <Route index element={<Songs />} />
                <Route path="albums" element={<AlbumsPage />} />
                <Route path="albums/:id" element={<AlbumDetailPage />} />
                <Route path="artists" element={<ArtistsPage />} />
                <Route path="artists/:id" element={<ArtistDetailPage />} />
                <Route
                    path="library"
                    element={
                        <ProtectedRoute>
                            <LibraryPage />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="settings"
                    element={
                        <ProtectedRoute>
                            <UserSettings />
                        </ProtectedRoute>
                    }
                />
            </Route>

            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />

            <Route
                path="/dashboard/*"
                element={
                    <ProtectedRoute adminOnly>
                        <Dashboard />
                    </ProtectedRoute>
                }
            />
        </Routes>
    );
};

function App() {
    return (
        <Router>
            <AuthProvider>
                <ToastProvider>
                    <MusicPlayerProvider>
                        <div className="min-h-screen bg-gray-50">
                            <AppRoutes />
                        </div>
                    </MusicPlayerProvider>
                </ToastProvider>
            </AuthProvider>
        </Router>
    );
}

export default App;