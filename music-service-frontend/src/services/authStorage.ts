const ACCESS_TOKEN_KEY = 'token';
const REFRESH_TOKEN_KEY = 'refreshToken';
const USER_KEY = 'user';
const AUTH_STATE_CHANGED_EVENT = 'auth-state-changed';

export interface StoredTokens {
  accessToken: string;
  refreshToken: string;
}

export const getAccessToken = () => localStorage.getItem(ACCESS_TOKEN_KEY);

export const getRefreshToken = () => localStorage.getItem(REFRESH_TOKEN_KEY);

export const getStoredUser = () => localStorage.getItem(USER_KEY);

export const setStoredUser = (user: string) => {
  localStorage.setItem(USER_KEY, user);
  notifyAuthStateChanged();
};

export const clearStoredUser = () => {
  localStorage.removeItem(USER_KEY);
  notifyAuthStateChanged();
};

export const setTokens = ({ accessToken, refreshToken }: StoredTokens) => {
  localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
  localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
  notifyAuthStateChanged();
};

export const clearTokens = () => {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
  notifyAuthStateChanged();
};

export const clearAuthStorage = () => {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
  notifyAuthStateChanged();
};

export const notifyAuthStateChanged = () => {
  window.dispatchEvent(new Event(AUTH_STATE_CHANGED_EVENT));
};

export const addAuthStateListener = (listener: () => void) => {
  window.addEventListener(AUTH_STATE_CHANGED_EVENT, listener);
  return () => window.removeEventListener(AUTH_STATE_CHANGED_EVENT, listener);
};
