import { useEffect, useState } from 'react';
import type { BookDto, PagedResponseDto } from '../types';
import { bookApi } from '../services/api';
import BookCard from '../components/BookCard';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { toast } from 'react-toastify';
import { BOOKS_PAGE_SIZE } from '../constants';

export default function HomePage() {
  const { t } = useTranslation();
  const [books, setBooks] = useState<PagedResponseDto<BookDto>>({
    items: [],
    pageNumber: 1,
    pageSize: BOOKS_PAGE_SIZE,
    totalItems: 0,
    totalPages: 0,
  });
  const navigate = useNavigate();

  useEffect(() => {
    bookApi
      .getBooks({ pageNumber: 1, pageSize: BOOKS_PAGE_SIZE, sortDescending: true })
      .then(setBooks)
      .catch(() => toast.error(t('FailedToLoadBooks')));
  }, [t]);

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">{t('Welcome')}</h1>
      <h2 className="text-xl font-semibold text-gray-800 mb-4">{t('RecentBooks')}</h2>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {books.items.map((book) => (
          <BookCard key={book.id} book={book} onClick={() => navigate(`/books/${book.id}`)} />
        ))}
      </div>
    </div>
  );
}