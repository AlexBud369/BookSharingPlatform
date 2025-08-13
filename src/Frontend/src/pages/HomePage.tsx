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
  const [errorShown, setErrorShown] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    setErrorShown(false);
    bookApi
      .getBooks({ pageNumber: 1, pageSize: BOOKS_PAGE_SIZE, sortDescending: true })
      .then(setBooks)
      .catch(() => {
        if (!errorShown) {
          toast.error(t('FailedToLoadBooks'));
          setErrorShown(true);
        }
      });
  }, [t]);

  return (
    <div className="container mx-auto p-4 sm:p-6">
      <h1 className="text-2xl sm:text-3xl font-bold text-gray-800 mb-6">{t('Welcome')}</h1>
      <h2 className="text-lg sm:text-xl font-semibold text-gray-800 mb-4">{t('RecentBooks')}</h2>
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4 sm:gap-6">
        {books.items.map((book) => (
          <BookCard key={book.id} book={book} onClick={() => navigate(`/books/${book.id}`)} />
        ))}
      </div>
    </div>
  );
}