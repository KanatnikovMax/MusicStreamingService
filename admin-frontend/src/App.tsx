import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { ToastProvider } from './contexts/ToastContext';
import Dashboard from './pages/Dashboard';
import Login from './pages/Login';
import ProtectedRoute from './components/ProtectedRoute';

function App() {
    return (
        <Router>
            <AuthProvider>
                <ToastProvider>
                    <div className="min-h-screen bg-gray-50">
                        <Routes>
                            <Route path="/login" element={<Login />} />
                            <Route
                                path="/dashboard/*"
                                element={
                                    <ProtectedRoute>
                                        <Dashboard />
                                    </ProtectedRoute>
                                }
                            />
                            <Route path="/" element={<Navigate to="/dashboard" replace />} />
                        </Routes>
                    </div>
                </ToastProvider>
            </AuthProvider>
        </Router>
    );
}

export default App;