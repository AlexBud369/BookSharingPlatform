import { Link, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuthStore } from '../store/authStore';
import { authApi } from '../services/api';
import Button from './Button';

export default function Header() {
  const { t, i18n } = useTranslation();
  const { user, logout } = useAuthStore();
  const navigate = useNavigate();

  const performLogout = async () => {
    await authApi.logout();
    logout();
    navigate('/login');
  };

  return (
    <header className="bg-blue-400 text-gray-100 p-4 sm:p-6 shadow-md">
      <nav className="container max-w-screen-xl mx-auto flex flex-col sm:flex-row justify-between items-center gap-4">
        <div className="flex flex-col sm:flex-row gap-4">
          <Link to="/" className="nav-link text-sm sm:text-base font-medium">
            {t('Home')}
          </Link>
          <Link to="/books" className="nav-link text-sm sm:text-base font-medium">
            {t('Books')}
          </Link>
          {user && (
            <Link to="/profile" className="hover:text-blue-200 transition-colors text-sm sm:text-base font-medium">
              {t('Profile')}
            </Link>
          )}
          {user?.role === 'Admin' && (
            <Link to="/admin" className="hover:text-blue-200 transition-colors text-sm sm:text-base font-medium">
              {t('Admin')}
            </Link>
          )}
        </div>
        <div className="flex gap-3 items-center">
          <button
            onClick={() => i18n.changeLanguage('en')}
            className={`px-2 py-1 rounded-lg font-medium text-sm sm:text-base transition-all duration-200 shadow-sm hover:shadow-md ${
              i18n.language === 'en' ? 'bg-blue-900 text-gray-100' : 'bg-blue-600 text-gray-100'
            }`}
          >
            EN
          </button>
          <button
            onClick={() => i18n.changeLanguage('ru')}
            className={`px-2 py-1 rounded-lg font-medium text-sm sm:text-base transition-all duration-200 shadow-sm hover:shadow-md ${
              i18n.language === 'ru' ? 'bg-blue-900 text-gray-100' : 'bg-blue-600 text-gray-100'
            }`}
          >
            RU
          </button>
          {user ? (
            <Button onClick={performLogout} variant="danger" className="px-3 py-1.5 text-sm sm:text-base">
              {t('Logout')}
            </Button>
          ) : (
            <Button variant="primary" onClick={() => navigate('/login')} className="px-3 py-1.5 text-sm sm:text-base">
              {t('Login')}
            </Button>
          )}
        </div>
      </nav>
    </header>
  );
}