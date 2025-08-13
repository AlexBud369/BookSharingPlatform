import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { toast } from 'react-toastify';
import { bookApi } from '../services/api';
import BookForm from '../components/BookForm';
import type { BookDto, BookUpdateDto } from '../types';

export default function EditBookPage() {
  const { t } = useTranslation();
  const { id } = useParams<{ id: string }>();
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

  const handleSubmit = async (data: BookUpdateDto) => {
    if (!id) return;
    try {
      await bookApi.updateBook(id, { ...data, tagIds: data.tags });
      if (data.coverImage) {
        await bookApi.uploadCover(id, data.coverImage[0]);
      }
      toast.success(t('BookUpdated'));
      navigate(`/books/${id}`);
    } catch (error: any) {
      toast.error(t(error.response?.data?.errors?.[0] || 'UnknownError'));
    }
  };

  if (!book) return <div className="text-center p-6 text-gray-700">{t('Loading')}</div>;

  const initialData: BookUpdateDto = {
    title: book.title,
    author: book.author,
    description: book.description,
    tags: book.tags.map((tag) => tag.id),
    coverImage: undefined,
  };

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">{t('EditBook')}</h1>
      <BookForm initialData={initialData} bookId={id} onSubmit={handleSubmit} />
    </div>
  );
}