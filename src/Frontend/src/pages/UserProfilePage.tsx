import { useEffect, useState } from 'react';
import type { UserDto, UserUpdateDto } from '../types';
import { userApi } from '../services/api';
import { useAuthStore } from '../store/authStore';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useTranslation } from 'react-i18next';
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
      userApi.getCurrentUser().then((data: UserDto) => {
        setProfile(data);
        reset({ username: data.username, email: data.email });
      });
    }
  }, [user, reset]);

  const updateUser = async (id: string, data: UserUpdateDto) => {
    const updatedUser = await userApi.updateUser(id, data);
    setProfile(updatedUser);
  };

  const onSubmit = async (data: UserUpdateDto) => {
    if (!user) return;
    await updateUser(user.id, data);
  };

  if (!profile) return <div className="text-center p-4 sm:p-6 text-gray-700">{t('Loading')}</div>;

  return (
    <div className="container mx-auto p-4 sm:p-6">
      <h1 className="text-2xl sm:text-3xl font-bold text-gray-800 mb-6 text-center">{t('MyProfile')}</h1>
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="space-y-4 max-w-md sm:max-w-lg mx-auto p-4 sm:p-6 bg-white rounded-lg shadow-md"
      >
        <div>
          <label className="block text-sm font-semibold text-gray-800">{t('Username')}</label>
          <input
            {...register('username')}
            className="w-full p-2 sm:p-3 border rounded-lg text-sm sm:text-base focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            placeholder={t('Username')}
          />
          {errors.username && <p className="text-red-600 mt-1">{t(errors.username.message!)}</p>}
        </div>
        <div>
          <label className="block text-sm font-semibold text-gray-800">{t('Email')}</label>
          <input
            {...register('email')}
            className="w-full p-2 sm:p-3 border rounded-lg text-sm sm:text-base focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            placeholder={t('Email')}
          />
          {errors.email && <p className="text-red-600 mt-1">{t(errors.email.message!)}</p>}
        </div>
        <div>
          <label className="block text-sm font-semibold text-gray-800">{t('Password')}</label>
          <input
            {...register('password')}
            type="password"
            className="w-full p-2 sm:p-3 border rounded-lg text-sm sm:text-base focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            placeholder={t('NewPasswordOptional')}
          />
          {errors.password && <p className="text-red-600 mt-1">{t(errors.password.message!)}</p>}
        </div>
        <Button type="submit" variant="primary" className="px-3 py-1.5 text-sm sm:text-base">
          {t('UpdateProfile')}
        </Button>
      </form>
    </div>
  );
}