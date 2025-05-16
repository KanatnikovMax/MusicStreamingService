import React, { useState } from 'react';
import { useAuth } from '../../contexts/AuthContext.tsx';
import { useToast } from '../../contexts/ToastContext.tsx';
import { 
  changeEmail, 
  changePassword, 
  changeUsername 
} from '../../services/userService.ts';

const UserSettings: React.FC = () => {
  const { user, logout } = useAuth();
  const { showToast } = useToast();
  
  const [activeTab, setActiveTab] = useState('email');
  
  // Email change state
  const [email, setEmail] = useState('');
  const [emailPassword, setEmailPassword] = useState('');
  const [isChangingEmail, setIsChangingEmail] = useState(false);
  
  // Username change state
  const [username, setUsername] = useState('');
  const [usernamePassword, setUsernamePassword] = useState('');
  const [isChangingUsername, setIsChangingUsername] = useState(false);
  
  // Password change state
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [isChangingPassword, setIsChangingPassword] = useState(false);

  const handleChangeEmail = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!email || !emailPassword) {
      showToast('Please fill in all fields', 'error');
      return;
    }
    
    setIsChangingEmail(true);
    try {
      await changeEmail(user?.username || '', {
        email,
        password: emailPassword
      });
      
      showToast('Email changed successfully. Please log in again.', 'success');
      logout();
    } catch {
      showToast('Failed to change email. Please check your password.', 'error');
    } finally {
      setIsChangingEmail(false);
    }
  };

  const handleChangeUsername = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!username || !usernamePassword) {
      showToast('Please fill in all fields', 'error');
      return;
    }
    
    setIsChangingUsername(true);
    try {
      await changeUsername(user?.username || '', {
        userName: username,
        password: usernamePassword
      });
      
      showToast('Username changed successfully. Please log in again.', 'success');
      logout();
    } catch {
      showToast('Failed to change username. Please check your password.', 'error');
    } finally {
      setIsChangingUsername(false);
    }
  };

  const handleChangePassword = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!currentPassword || !newPassword || !confirmPassword) {
      showToast('Please fill in all fields', 'error');
      return;
    }
    
    if (newPassword !== confirmPassword) {
      showToast('New passwords do not match', 'error');
      return;
    }
    
    setIsChangingPassword(true);
    try {
      await changePassword(user?.username || '', {
        currentPassword,
        newPassword
      });
      
      showToast('Password changed successfully. Please log in again.', 'success');
      logout();
    } catch {
      showToast('Failed to change password. Please check your current password.', 'error');
    } finally {
      setIsChangingPassword(false);
    }
  };

  return (
    <div>
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Account Settings</h1>
      </div>

      <div className="bg-white shadow rounded-lg overflow-hidden">
        <div className="md:flex min-h-[400px]">
          <div className="md:w-1/4 bg-gray-50 p-6 border-r border-gray-200">
            <div className="space-y-1">
              <button
                className={`block w-full text-left px-3 py-2 rounded-md ${
                  activeTab === 'email'
                    ? 'bg-indigo-100 text-indigo-700 font-medium'
                    : 'text-gray-700 hover:bg-gray-100'
                }`}
                onClick={() => setActiveTab('email')}
              >
                Change Email
              </button>
              <button
                className={`block w-full text-left px-3 py-2 rounded-md ${
                  activeTab === 'username'
                    ? 'bg-indigo-100 text-indigo-700 font-medium'
                    : 'text-gray-700 hover:bg-gray-100'
                }`}
                onClick={() => setActiveTab('username')}
              >
                Change Username
              </button>
              <button
                className={`block w-full text-left px-3 py-2 rounded-md ${
                  activeTab === 'password'
                    ? 'bg-indigo-100 text-indigo-700 font-medium'
                    : 'text-gray-700 hover:bg-gray-100'
                }`}
                onClick={() => setActiveTab('password')}
              >
                Change Password
              </button>
            </div>
          </div>
          
          <div className="md:w-3/4 p-6">
            {activeTab === 'email' && (
              <div>
                <h2 className="text-xl font-semibold mb-4">Change Email</h2>
                <form onSubmit={handleChangeEmail}>
                  <div className="mb-4">
                    <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1">
                      New Email
                    </label>
                    <input
                      type="email"
                      id="email"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                      required
                    />
                  </div>
                  
                  <div className="mb-6">
                    <label htmlFor="emailPassword" className="block text-sm font-medium text-gray-700 mb-1">
                      Current Password
                    </label>
                    <input
                      type="password"
                      id="emailPassword"
                      value={emailPassword}
                      onChange={(e) => setEmailPassword(e.target.value)}
                      className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                      required
                    />
                  </div>
                  
                  <button
                    type="submit"
                    disabled={isChangingEmail}
                    className={`w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 ${
                      isChangingEmail ? 'opacity-70 cursor-not-allowed' : ''
                    }`}
                  >
                    {isChangingEmail ? 'Changing email...' : 'Change Email'}
                  </button>
                </form>
              </div>
            )}
            
            {activeTab === 'username' && (
              <div>
                <h2 className="text-xl font-semibold mb-4">Change Username</h2>
                <form onSubmit={handleChangeUsername}>
                  <div className="mb-4">
                    <label htmlFor="username" className="block text-sm font-medium text-gray-700 mb-1">
                      New Username
                    </label>
                    <input
                      type="text"
                      id="username"
                      value={username}
                      onChange={(e) => setUsername(e.target.value)}
                      className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                      required
                    />
                  </div>
                  
                  <div className="mb-6">
                    <label htmlFor="usernamePassword" className="block text-sm font-medium text-gray-700 mb-1">
                      Current Password
                    </label>
                    <input
                      type="password"
                      id="usernamePassword"
                      value={usernamePassword}
                      onChange={(e) => setUsernamePassword(e.target.value)}
                      className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                      required
                    />
                  </div>
                  
                  <button
                    type="submit"
                    disabled={isChangingUsername}
                    className={`w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 ${
                      isChangingUsername ? 'opacity-70 cursor-not-allowed' : ''
                    }`}
                  >
                    {isChangingUsername ? 'Changing username...' : 'Change Username'}
                  </button>
                </form>
              </div>
            )}
            
            {activeTab === 'password' && (
              <div>
                <h2 className="text-xl font-semibold mb-4">Change Password</h2>
                <form onSubmit={handleChangePassword}>
                  <div className="mb-4">
                    <label htmlFor="currentPassword" className="block text-sm font-medium text-gray-700 mb-1">
                      Current Password
                    </label>
                    <input
                      type="password"
                      id="currentPassword"
                      value={currentPassword}
                      onChange={(e) => setCurrentPassword(e.target.value)}
                      className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                      required
                    />
                  </div>
                  
                  <div className="mb-4">
                    <label htmlFor="newPassword" className="block text-sm font-medium text-gray-700 mb-1">
                      New Password
                    </label>
                    <input
                      type="password"
                      id="newPassword"
                      value={newPassword}
                      onChange={(e) => setNewPassword(e.target.value)}
                      className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                      required
                    />
                  </div>
                  
                  <div className="mb-6">
                    <label htmlFor="confirmPassword" className="block text-sm font-medium text-gray-700 mb-1">
                      Confirm New Password
                    </label>
                    <input
                      type="password"
                      id="confirmPassword"
                      value={confirmPassword}
                      onChange={(e) => setConfirmPassword(e.target.value)}
                      className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                      required
                    />
                  </div>
                  
                  <button
                    type="submit"
                    disabled={isChangingPassword}
                    className={`w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 ${
                      isChangingPassword ? 'opacity-70 cursor-not-allowed' : ''
                    }`}
                  >
                    {isChangingPassword ? 'Changing password...' : 'Change Password'}
                  </button>
                </form>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default UserSettings;