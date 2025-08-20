import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import type { BookDto } from '../types';
import { bookApi } from '../services/api';
import { useAuthStore } from '../store/authStore';
import { useTranslation } from 'react-i18next';
import Button from '../components/Button';

export default function BookDetailsPage() {
  const { t } = useTranslation();
  const { id } = useParams<{ id: string }>();
  const { user } = useAuthStore();
  const navigate = useNavigate();
  const [book, setBook] = useState<BookDto | null>(null);

  useEffect(() => {
    if (id) {
      bookApi.getBookById(id).then(setBook);
    }
  }, [id]);

  const deleteBook = async (id: string) => {
    await bookApi.deleteBook(id);
  };

  const confirmAndDelete = async (id: string) => {
    if (window.confirm(t('ConfirmDelete'))) {
      await deleteBook(id);
      navigate('/books');
    }
  };

  if (!book) return <div className="text-center p-4 sm:p-6 text-gray-700">{t('Loading')}</div>;

  const canEdit = user && (user.role === 'Admin' || user.id === book.createdByUserId);

  return (
    <div className="container mx-auto p-4 sm:p-6 bg-white rounded-lg shadow-md">
      <h1 className="text-2xl sm:text-3xl font-bold text-gray-800 mb-6">{book.title}</h1>
      <p className="text-gray-700 mb-4">
        <strong>{t('Author')}:</strong> {book.author}
      </p>
      {book.description && <p className="text-gray-700 mb-4">{book.description}</p>}
      {book.coverImageUrl && (
        <img
          src={book.coverImageUrl}
          alt={book.title}
          className="w-full max-w-md sm:max-w-lg h-auto object-cover rounded-lg mb-4"
        />
      )}
      <div className="mb-4 flex flex-wrap gap-2">
        {book.tags.map((tag, index) => (
          <span
            key={index}
            className="inline-block bg-blue-100 text-blue-900 text-sm font-medium px-2 sm:px-3 py-1 rounded-full"
          >
            {tag.tagName}
          </span>
        ))}
      </div>
      {canEdit && (
        <div className="flex gap-4">
          <Button variant="primary" onClick={() => navigate(`/books/${id}/edit`)} className="px-3 py-1.5 text-sm">
            {t('Edit')}
          </Button>
          <Button variant="danger" onClick={() => confirmAndDelete(id!)} className="px-3 py-1.5 text-sm">
            {t('Delete')}
          </Button>
        </div>
      )}
    </div>
  );
}