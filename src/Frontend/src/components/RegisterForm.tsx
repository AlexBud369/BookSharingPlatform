import { useForm } from 'react-hook-form';
import type { RegisterDto } from '../types';
import { authApi } from '../services/api';
import { useAuthStore } from '../store/authStore';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { toast } from 'react-toastify';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import Button from './Button';

const schema = yup.object({
  username: yup
    .string()
    .required('UsernameRequired')
    .max(50, 'UsernameTooLong')
    .matches(/^[a-zA-Z0-9_]+$/, 'InvalidUsernameFormat'),
  email: yup.string().required('EmailRequired').email('InvalidEmailFormat').max(100, 'EmailTooLong'),
  password: yup
    .string()
    .required('PasswordRequired')
    .min(6, 'PasswordTooShort')
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

  const onSubmit = async (data: RegisterDto) => {
    try {
      const response = await authApi.register(data);
      setAuth(response);
      toast.success(t('RegistrationSuccessful'));
      navigate('/');
    } catch (error: any) {
      toast.error(t(error.response?.data?.errors?.[0] || 'RegistrationFailed'));
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div>
        <label className="block text-sm font-medium">{t('Username')}</label>
        <input
          {...register('username')}
          placeholder={t('Username')}
          className="w-full p-2 border rounded"
        />
        {errors.username && <p className="text-red-500">{t(errors.username.message!)}</p>}
      </div>
      <div>
        <label className="block text-sm font-medium">{t('Email')}</label>
        <input
          {...register('email')}
          placeholder={t('Email')}
          className="w-full p-2 border rounded"
        />
        {errors.email && <p className="text-red-500">{t(errors.email.message!)}</p>}
      </div>
      <div>
        <label className="block text-sm font-medium">{t('Password')}</label>
        <input
          {...register('password')}
          placeholder={t('Password')}
          type="password"
          className="w-full p-2 border rounded"
        />
        {errors.password && <p className="text-red-500">{t(errors.password.message!)}</p>}
      </div>
      <Button type="submit" variant="primary">
        {t('Register')}
      </Button>
    </form>
  );
}