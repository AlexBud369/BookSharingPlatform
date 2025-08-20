import { useEffect, useState } from 'react';
import type { UserDto, PagedResponseDto, BookDto } from '../types';
import { userApi, bookApi } from '../services/api';
import { useAuthStore } from '../store/authStore';
import { useTranslation } from 'react-i18next';
import Button from '../components/Button';
import { USERS_PAGE_SIZE, BOOKS_PAGE_SIZE, DEFAULT_PAGE_NUMBER, DEFAULT_TOTAL_ITEMS, DEFAULT_TOTAL_PAGES } from '../constants';

export default function AdminPanel() {
  const { t } = useTranslation();
  const { user } = useAuthStore();
  const [users, setUsers] = useState<PagedResponseDto<UserDto>>({
    items: [],
    pageNumber: DEFAULT_PAGE_NUMBER,
    pageSize: USERS_PAGE_SIZE,
    totalItems: DEFAULT_TOTAL_ITEMS,
    totalPages: DEFAULT_TOTAL_PAGES,
  });
  const [books, setBooks] = useState<PagedResponseDto<BookDto>>({
    items: [],
    pageNumber: DEFAULT_PAGE_NUMBER,
    pageSize: BOOKS_PAGE_SIZE,
    totalItems: DEFAULT_TOTAL_ITEMS,
    totalPages: DEFAULT_TOTAL_PAGES,
  });

  useEffect(() => {
    if (user?.role === 'Admin') {
      userApi.getUsers({ pageNumber: DEFAULT_PAGE_NUMBER, pageSize: USERS_PAGE_SIZE }).then(setUsers);
      bookApi.getBooks({ pageNumber: DEFAULT_PAGE_NUMBER, pageSize: BOOKS_PAGE_SIZE, sortDescending: false }).then(setBooks);
    }
  }, [user]);

  const blockUser = async (userId: string, currentUserId: string) => {
    await userApi.blockUser(userId, currentUserId);
    setUsers((prev) => ({
      ...prev,
      items: prev.items.map((u) => (u.id === userId ? { ...u, isBlocked: true } : u)),
    }));
  };

  const deleteBook = async (bookId: string) => {
    await bookApi.deleteBook(bookId);
    setBooks((prev) => ({
      ...prev,
      items: prev.items.filter((b) => b.id !== bookId),
    }));
  };

  const confirmAndDeleteBook = async (bookId: string) => {
    if (window.confirm(t('ConfirmDelete'))) {
      await deleteBook(bookId);
    }
  };

  const handleBlockUser = async (userId: string) => {
    if (!user) return;
    await blockUser(userId, user.id);
  };

  if (user?.role !== 'Admin') return <div className="text-red-600 text-center p-4 sm:p-6">{t('AdminOnlyAccess')}</div>;

  return (
    <div className="container mx-auto p-4 sm:p-6">
      <h1 className="text-2xl sm:text-3xl font-bold text-gray-800 mb-6">{t('AdminPanel')}</h1>
      <h2 className="text-lg sm:text-xl font-semibold text-gray-800 mb-4">{t('Users')}</h2>
      <div className="overflow-x-auto bg-white rounded-lg shadow-md">
        <ul className="divide-y divide-gray-200">
          {users.items.map((u) => (
            <li key={u.id} className="p-4 flex justify-between items-center">
              <span className="text-gray-700">
                {u.username} ({u.email}) - {t('Role')}: {u.role}
              </span>
              <Button
                className="ml-4 px-3 py-1.5 text-sm"
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
      <h2 className="text-lg sm:text-xl font-semibold text-gray-800 mb-4 mt-6">{t('Books')}</h2>
      <div className="overflow-x-auto bg-white rounded-lg shadow-md">
        <ul className="divide-y divide-gray-200">
          {books.items.map((b) => (
            <li key={b.id} className="p-4 flex justify-between items-center">
              <span className="text-gray-700">
                {b.title} {t('By')} {b.author}
              </span>
              <Button className="ml-4 px-3 py-1.5 text-sm" variant="danger" onClick={() => confirmAndDeleteBook(b.id)}>
                {t('Delete')}
              </Button>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}