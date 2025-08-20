import RegisterForm from '../components/RegisterForm';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

export default function RegisterPage() {
  const { t } = useTranslation();

  return (
    <div className="container mx-auto p-4 sm:p-6">
      <h1 className="text-2xl sm:text-3xl font-bold text-gray-800 mb-6 text-center">{t('Register')}</h1>
      <RegisterForm />
      <p className="mt-6 text-center text-gray-700 text-sm sm:text-base">
        {t('HaveAccount')} <Link to="/login" className="text-blue-700 hover:underline">{t('Login')}</Link>
      </p>
    </div>
  );
}