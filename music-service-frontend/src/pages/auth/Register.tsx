import React from 'react';
import { Link } from 'react-router-dom';
import RegisterForm from '../../components/RegisterForm.tsx';
import { Music } from 'lucide-react';

const Register: React.FC = () => {
  return (
      <div className="min-h-screen bg-gradient-to-b from-gray-900 to-gray-800 flex items-center justify-center px-4 sm:px-6 lg:px-8">
        <div className="max-w-md w-full space-y-8 bg-white rounded-xl shadow-2xl p-8">
          <div className="text-center">
            <div className="flex justify-center">
              <div className="h-16 w-16 rounded-full bg-indigo-100 flex items-center justify-center">
                <Music size={32} className="text-indigo-600" />
              </div>
            </div>
            <h2 className="mt-6 text-3xl font-extrabold text-gray-900">
              Create your account
            </h2>
            <p className="mt-2 text-sm text-gray-600">
              Or{' '}
              <Link to="/login" className="font-medium text-indigo-600 hover:text-indigo-500">
                sign in to your existing account
              </Link>
            </p>
          </div>
          <RegisterForm />
        </div>
      </div>
  );
};

export default Register;