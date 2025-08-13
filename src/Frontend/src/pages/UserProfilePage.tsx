import { useEffect, useState } from 'react';
import type { UserDto, UserUpdateDto } from '../types';
import { userApi } from '../services/api';
import { useAuthStore } from '../store/authStore';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useTranslation } from 'react-i18next';
import { toast } from 'react-toastify';
import Button from '../components/Button';
import { MAX_USERNAME_LENGTH, MAX_EMAIL_LENGTH, MIN_PASSWORD_LENGTH } from '../constants';

const schema = yup.object({
  username: yup.string().max(MAX_USERNAME_LENGTH, 'UsernameTooLong').optional(),
  email: yup.string().email('InvalidEmailFormat').max(MAX_EMAIL_LENGTH, 'EmailTooLong').optional(),
  password: yup
    .string()
    .min(MIN_PASSWORD_LENGTH, 'PasswordTooShort')
    .matches(/[A-Z]/, 'PasswordRequiresUppercase')
    .matches(/[0-9]/, 'PasswordRequiresNumber')
    .optional(),
});

export default function UserProfilePage() {
  const { t } = useTranslation();
  const { user } = useAuthStore();
  const [profile, setProfile] = useState<UserDto | null>(null);
  const { register, handleSubmit, reset, formState: { errors } } = useForm<UserUpdateDto>({
    resolver: yupResolver(schema),
    defaultValues: {
      username: user?.username,
      email: user?.email,
      password: undefined,
    },
  });

  useEffect(() => {
    if (user) {
      userApi
        .getCurrentUser()
        .then((data: UserDto) => {
          setProfile(data);
          reset({ username: data.username, email: data.email });
        })
        .catch(() => toast.error(t('FailedToLoadProfile')));
    }
  }, [user, reset, t]);

  const onSubmit = async (data: UserUpdateDto) => {
    if (!user) return;
    try {
      const updatedUser = await userApi.updateUser(user.id, data);
      setProfile(updatedUser);
      toast.success(t('ProfileUpdated'));
    } catch (error: any) {
      toast.error(t(error.response?.data?.errors?.[0] || 'FailedToUpdateUser'));
    }
  };

  if (!profile) return <div className="text-center p-6 text-gray-700">{t('Loading')}</div>;

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-3xl font-bold text-gray-800 mb-6 text-center">{t('MyProfile')}</h1>
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="space-y-6 max-w-md md:max-w-lg mx-auto p-6 bg-white rounded-lg shadow-md"
      >
        <div>
          <label className="block text-sm font-semibold text-gray-800">{t('Username')}</label>
          <input
            {...register('username')}
            className="w-full p-3 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            placeholder={t('Username')}
          />
          {errors.username && <p className="text-red-600 mt-1">{t(errors.username.message!)}</p>}
        </div>
        <div>
          <label className="block text-sm font-semibold text-gray-800">{t('Email')}</label>
          <input
            {...register('email')}
            className="w-full p-3 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            placeholder={t('Email')}
          />
          {errors.email && <p className="text-red-600 mt-1">{t(errors.email.message!)}</p>}
        </div>
        <div>
          <label className="block text-sm font-semibold text-gray-800">{t('Password')}</label>
          <input
            {...register('password')}
            type="password"
            className="w-full p-3 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            placeholder={t('NewPasswordOptional')}
          />
          {errors.password && <p className="text-red-600 mt-1">{t(errors.password.message!)}</p>}
        </div>
        <Button type="submit" variant="primary">
          {t('UpdateProfile')}
        </Button>
      </form>
    </div>
  );
}