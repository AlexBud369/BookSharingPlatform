import { useEffect, useState } from 'react';
import type { UserDto, PagedResponseDto, BookDto } from '../types';
import { userApi, bookApi } from '../services/api';
import { useAuthStore } from '../store/authStore';
import { useTranslation } from 'react-i18next';
import { toast } from 'react-toastify';
import Button from '../components/Button';
import { USERS_PAGE_SIZE, BOOKS_PAGE_SIZE } from '../constants';

export default function AdminPanel() {
  const { t } = useTranslation();
  const { user } = useAuthStore();
  const [users, setUsers] = useState<PagedResponseDto<UserDto>>({
    items: [],
    pageNumber: 1,
    pageSize: USERS_PAGE_SIZE,
    totalItems: 0,
    totalPages: 0,
  });
  const [books, setBooks] = useState<PagedResponseDto<BookDto>>({
    items: [],
    pageNumber: 1,
    pageSize: BOOKS_PAGE_SIZE,
    totalItems: 0,
    totalPages: 0,
  });

  useEffect(() => {
    if (user?.role === 'Admin') {
      userApi.getUsers({ pageNumber: 1, pageSize: USERS_PAGE_SIZE }).then(setUsers).catch(() => toast.error(t('FailedToLoadUsers')));
      bookApi
        .getBooks({ pageNumber: 1, pageSize: BOOKS_PAGE_SIZE, sortDescending: false })
        .then(setBooks)
        .catch(() => toast.error(t('FailedToLoadBooks')));
    }
  }, [user, t]);

  const handleBlockUser = async (userId: string) => {
    try {
      await userApi.blockUser(userId, user!.id);
      setUsers((prev) => ({
        ...prev,
        items: prev.items.map((u) => (u.id === userId ? { ...u, isBlocked: true } : u)),
      }));
      toast.success(t('UserBlocked'));
    } catch (error: any) {
      toast.error(t(error.response?.data?.errors?.[0] || 'FailedToBlockUser'));
    }
  };

  const handleDeleteBook = async (bookId: string) => {
    if (window.confirm(t('ConfirmDelete'))) {
      try {
        await bookApi.deleteBook(bookId);
        setBooks((prev) => ({
          ...prev,
          items: prev.items.filter((b) => b.id !== bookId),
        }));
        toast.success(t('BookDeleted'));
      } catch (error: any) {
        toast.error(t(error.response?.data?.errors?.[0] || 'FailedToDeleteBook'));
      }
    }
  };

  if (user?.role !== 'Admin') return <div className="text-red-600 text-center p-6">{t('AdminOnlyAccess')}</div>;

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">{t('AdminPanel')}</h1>
      <h2 className="text-xl font-semibold text-gray-800 mb-4">{t('Users')}</h2>
      <div className="overflow-x-auto bg-white rounded-lg shadow-md">
        <ul className="divide-y divide-gray-200">
          {users.items.map((u) => (
            <li key={u.id} className="p-4 flex justify-between items-center">
              <span className="text-gray-700">
                {u.username} ({u.email}) - {t('Role')}: {u.role}
              </span>
              <Button
                className="ml-4"
                variant="danger"
                onClick={() => handleBlockUser(u.id)}
                disabled={u.isBlocked}
              >
                {t('Block')}
              </Button>
            </li>
          ))}
        </ul>
      </div>
      <h2 className="text-xl font-semibold text-gray-800 mb-4 mt-6">{t('Books')}</h2>
      <div className="overflow-x-auto bg-white rounded-lg shadow-md">
        <ul className="divide-y divide-gray-200">
          {books.items.map((b) => (
            <li key={b.id} className="p-4 flex justify-between items-center">
              <span className="text-gray-700">
                {b.title} {t('By')} {b.author}
              </span>
              <Button className="ml-4" variant="danger" onClick={() => handleDeleteBook(b.id)}>
                {t('Delete')}
              </Button>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}