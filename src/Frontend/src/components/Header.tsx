import { Link, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuthStore } from '../store/authStore';
import { toast } from 'react-toastify';
import { authApi } from '../services/api';
import Button from './Button';

export default function Header() {
  const { t, i18n } = useTranslation();
  const { user, logout } = useAuthStore();
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
      await authApi.logout();
      logout();
      toast.success(t('LogoutSuccessful'));
      navigate('/login');
    } catch (error: any) {
      toast.error(t(error.response?.data?.errors?.[0] || 'UnknownError'));
    }
  };

  return (
    <header className="bg-blue-700 text-gray-100 p-6 shadow-md">
      <nav className="container mx-auto flex justify-between items-center">
        <div className="flex gap-6">
          <Link to="/" className="hover:text-blue-200 transition-colors text-lg font-medium">
            {t('Home')}
          </Link>
          <Link to="/books" className="hover:text-blue-200 transition-colors text-lg font-medium">
            {t('Books')}
          </Link>
          {user && (
            <Link to="/profile" className="hover:text-blue-200 transition-colors text-lg font-medium">
              {t('Profile')}
            </Link>
          )}
          {user?.role === 'Admin' && (
            <Link to="/admin" className="hover:text-blue-200 transition-colors text-lg font-medium">
              {t('Admin')}
            </Link>
          )}
        </div>
        <div className="flex gap-4 items-center">
          <button
            onClick={() => i18n.changeLanguage('en')}
            className={`px-3 py-1.5 rounded-lg font-medium text-sm transition-all duration-200 shadow-sm hover:shadow-md ${
              i18n.language === 'en' ? 'bg-blue-900 text-white' : 'bg-blue-600 text-gray-100'
            }`}
          >
            EN
          </button>
          <button
            onClick={() => i18n.changeLanguage('ru')}
            className={`px-3 py-1.5 rounded-lg font-medium text-sm transition-all duration-200 shadow-sm hover:shadow-md ${
              i18n.language === 'ru' ? 'bg-blue-900 text-white' : 'bg-blue-600 text-gray-100'
            }`}
          >
            RU
          </button>
          {user ? (
            <Button onClick={handleLogout} variant="danger">
              {t('Logout')}
            </Button>
          ) : (
            <Link
              to="/login"
              className="px-6 py-3 bg-blue-600 text-gray-100 rounded-lg font-semibold text-sm hover:bg-blue-700 transition-all shadow-md"
            >
              {t('Login')}
            </Link>
          )}
        </div>
      </nav>
    </header>
  );
}