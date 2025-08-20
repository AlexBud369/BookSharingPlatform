import { useForm } from 'react-hook-form';
import type { RegisterDto } from '../types';
import { authApi } from '../services/api';
import { useAuthStore } from '../store/authStore';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import Button from './Button';
import { MAX_USERNAME_LENGTH, MAX_EMAIL_LENGTH, MIN_PASSWORD_LENGTH } from '../constants';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';

const schema = yup.object({
  username: yup
    .string()
    .required('UsernameRequired')
    .max(MAX_USERNAME_LENGTH, 'UsernameTooLong')
    .matches(/^[a-zA-Z0-9_]+$/, 'InvalidUsernameFormat'),
  email: yup.string().required('EmailRequired').email('InvalidEmailFormat').max(MAX_EMAIL_LENGTH, 'EmailTooLong'),
  password: yup
    .string()
    .required('PasswordRequired')
    .min(MIN_PASSWORD_LENGTH, 'PasswordTooShort')
    .matches(/[A-Z]/, 'PasswordRequiresUppercase')
    .matches(/[0-9]/, 'PasswordRequiresNumber'),
});

export default function RegisterForm() {
  const { t } = useTranslation();
  const { register, handleSubmit, formState: { errors } } = useForm<RegisterDto>({
    resolver: yupResolver(schema),
  });
  const { setAuth } = useAuthStore();
  const navigate = useNavigate();

  const performRegister = async (data: RegisterDto) => {
    const response = await authApi.register(data);
    setAuth(response);
    navigate('/');
  };

  return (
    <form
      onSubmit={handleSubmit(performRegister)}
      className="space-y-6 max-w-md md:max-w-lg mx-auto p-4 sm:p-6 bg-white rounded-lg shadow-md"
    >
      <div>
        <label className="block text-sm font-semibold text-gray-800">{t('Username')}</label>
        <input
          {...register('username')}
          placeholder={t('Username')}
          className="w-full p-2 sm:p-3 border rounded-lg text-sm sm:text-base focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        />
        {errors.username && <p className="text-red-600 mt-1">{t(errors.username.message!)}</p>}
      </div>
      <div>
        <label className="block text-sm font-semibold text-gray-800">{t('Email')}</label>
        <input
          {...register('email')}
          placeholder={t('Email')}
          className="w-full p-2 sm:p-3 border rounded-lg text-sm sm:text-base focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        />
        {errors.email && <p className="text-red-600 mt-1">{t(errors.email.message!)}</p>}
      </div>
      <div>
        <label className="block text-sm font-semibold text-gray-800">{t('Password')}</label>
        <input
          {...register('password')}
          placeholder={t('Password')}
          type="password"
          className="w-full p-2 sm:p-3 border rounded-lg text-sm sm:text-base focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        />
        {errors.password && <p className="text-red-600 mt-1">{t(errors.password.message!)}</p>}
      </div>
      <Button type="submit" variant="primary" className="px-3 py-1.5 text-sm sm:text-base">
        {t('Register')}
      </Button>
    </form>
  );
}