import React, { createContext, useContext, useEffect, useState } from 'react';
import { login as apiLogin } from '../services/authService';
import {
  addAuthStateListener,
  clearAuthStorage,
  getAccessToken,
  getStoredUser,
  setStoredUser,
  setTokens
} from '../services/authStorage';

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

const decodeJwtPayload = (token: string) => JSON.parse(atob(token.split('.')[1]));

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
    const syncAuthState = () => {
      const storedToken = getAccessToken();
      const storedUser = getStoredUser();

      if (storedToken && storedUser) {
        const parsedUser = JSON.parse(storedUser) as User;
        setToken(storedToken);
        setUser(parsedUser);
        setIsAuthenticated(true);
        setIsAdmin(parsedUser.role === 'admin');
      } else {
        setToken(null);
        setUser(null);
        setIsAuthenticated(false);
        setIsAdmin(false);
      }

      setLoading(false);
    };

    syncAuthState();
    return addAuthStateListener(syncAuthState);
  }, []);

  const handleLogin = async (username: string, password: string) => {
    setLoading(true);
    try {
      const { accessToken, refreshToken } = await apiLogin(username, password);

      if (accessToken && refreshToken) {
        const payload = decodeJwtPayload(accessToken);
        const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'user';
        const userId = payload.sub;

        const nextUser = {
          id: userId,
          username,
          role
        };

        setTokens({ accessToken, refreshToken });
        setStoredUser(JSON.stringify(nextUser));
      }
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    clearAuthStorage();
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
