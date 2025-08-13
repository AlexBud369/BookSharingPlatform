import LoginForm from '../components/LoginForm';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

export default function LoginPage() {
  const { t } = useTranslation();

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-3xl font-bold text-gray-800 mb-6 text-center">{t('Login')}</h1>
      <LoginForm />
      <p className="mt-6 text-center text-gray-700">
        {t('NoAccount')} <Link to="/register" className="text-blue-700 hover:underline">{t('Register')}</Link>
      </p>
    </div>
  );
}