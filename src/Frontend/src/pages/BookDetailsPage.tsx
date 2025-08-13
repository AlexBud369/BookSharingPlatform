import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import type { BookDto } from '../types';
import { bookApi } from '../services/api';
import { useAuthStore } from '../store/authStore';
import { useTranslation } from 'react-i18next';
import { toast } from 'react-toastify';
import Button from '../components/Button';

export default function BookDetailsPage() {
  const { t } = useTranslation();
  const { id } = useParams<{ id: string }>();
  const { user } = useAuthStore();
  const navigate = useNavigate();
  const [book, setBook] = useState<BookDto | null>(null);

  useEffect(() => {
    if (id) {
      bookApi
        .getBookById(id)
        .then(setBook)
        .catch(() => toast.error(t('BookNotFound', { 0: id })));
    }
  }, [id, t]);

  const handleDelete = async () => {
    if (id && window.confirm(t('ConfirmDelete'))) {
      try {
        await bookApi.deleteBook(id);
        toast.success(t('BookDeleted'));
        navigate('/books');
      } catch (error: any) {
        toast.error(t(error.response?.data?.errors?.[0] || 'FailedToDeleteBook'));
      }
    }
  };

  if (!book) return <div className="text-center p-6 text-gray-700">{t('Loading')}</div>;

  const canEdit = user && (user.role === 'Admin' || user.id === book.createdByUserId);

  return (
    <div className="container mx-auto p-6 bg-white rounded-lg shadow-md">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">{book.title}</h1>
      <p className="text-gray-700 mb-4">
        <strong>{t('Author')}:</strong> {book.author}
      </p>
      {book.description && <p className="text-gray-700 mb-4">{book.description}</p>}
      {book.coverImageUrl && (
        <img
          src={book.coverImageUrl}
          alt={book.title}
          className="w-full max-w-lg h-auto object-cover rounded-lg mb-4"
        />
      )}
      <div className="mb-4 flex flex-wrap gap-2">
        {book.tags.map((tag, index) => (
          <span
            key={index}
            className="inline-block bg-blue-100 text-blue-900 text-sm font-medium px-3 py-1 rounded-full"
          >
            {tag.tagName}
          </span>
        ))}
      </div>
      {canEdit && (
        <div className="flex gap-4">
          <Button variant="primary" onClick={() => navigate(`/books/${id}/edit`)}>
            {t('Edit')}
          </Button>
          <Button variant="danger" onClick={handleDelete}>
            {t('Delete')}
          </Button>
        </div>
      )}
    </div>
  );
}