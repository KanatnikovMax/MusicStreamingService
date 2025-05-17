import React, { createContext, useState, useContext, useEffect } from 'react';
import { login as apiLogin } from '../services/authService';

interface User {
  id: string;
  username: string;
  role?: string;
}

interface AuthContextType {
  isAuthenticated: boolean;
  user: User | null;
  token: string | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
  loading: boolean;
  isAdmin: boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [isAdmin, setIsAdmin] = useState<boolean>(false);

  useEffect(() => {
    const storedToken = localStorage.getItem('token');
    const storedUser = localStorage.getItem('user');

    if (storedToken && storedUser) {
      const parsedUser = JSON.parse(storedUser);
      setToken(storedToken);
      setUser(parsedUser);
      setIsAuthenticated(true);
      setIsAdmin(parsedUser.role === 'admin');
    }

    setLoading(false);
  }, []);

  const handleLogin = async (username: string, password: string) => {
    setLoading(true);
    try {
      const response = await apiLogin(username, password);
      const { accessToken } = response;

      if (accessToken) {
        // Parse JWT to get user info
        const payload = JSON.parse(atob(accessToken.split('.')[1]));

        const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || "user";
        const userId = payload.sub;

        const user = {
          id: userId,
          username,
          role
        };

        localStorage.setItem('token', accessToken);
        localStorage.setItem('user', JSON.stringify(user));
        setToken(accessToken);
        setUser(user);
        setIsAuthenticated(true);
        setIsAdmin(role === 'admin');
      }
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setToken(null);
    setUser(null);
    setIsAuthenticated(false);
    setIsAdmin(false);
  };

  const value = {
    isAuthenticated,
    user,
    token,
    login: handleLogin,
    logout: handleLogout,
    loading,
    isAdmin
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};