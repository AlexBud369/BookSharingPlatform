import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
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
      bookApi.getBookById(id).then(setBook);
    }
  }, [id]);

  const updateBook = async (id: string, data: BookUpdateDto) => {
    await bookApi.updateBook(id, { ...data, tagIds: data.tags });
  };

  const uploadCoverImage = async (id: string, coverImage?: FileList) => {
    if (coverImage && coverImage.length > 0) {
      await bookApi.uploadCover(id, coverImage[0]);
    }
  };

  const navigateToBook = (id: string) => {
    navigate(`/books/${id}`);
  };

  const handleSubmit = async (data: BookUpdateDto) => {
    if (!id) return;
    await updateBook(id, data);
    await uploadCoverImage(id, data.coverImage);
    navigateToBook(id);
  };

  if (!book) return <div className="text-center p-4 sm:p-6 text-gray-700">{t('Loading')}</div>;

  const initialData: BookUpdateDto = {
    title: book.title,
    author: book.author,
    description: book.description,
    tags: book.tags.map((tag) => tag.id),
    coverImage: undefined,
  };

  return (
    <div className="container mx-auto p-4 sm:p-6">
      <h1 className="text-2xl sm:text-3xl font-bold text-gray-800 mb-6">{t('EditBook')}</h1>
      <BookForm initialData={initialData} bookId={id} onSubmit={handleSubmit} />
    </div>
  );
}