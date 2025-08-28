import { useEffect, useState } from 'react';
import type { BookDto, PagedResponseDto } from '../types';
import { bookApi } from '../services/api';
import BookCard from '../components/BookCard';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { BOOKS_PAGE_SIZE, DEFAULT_PAGE_NUMBER, DEFAULT_TOTAL_ITEMS, DEFAULT_TOTAL_PAGES } from '../constants';

export default function HomePage() {
  const { t } = useTranslation();
  const [books, setBooks] = useState<PagedResponseDto<BookDto>>({
    items: [],
    pageNumber: DEFAULT_PAGE_NUMBER,
    pageSize: BOOKS_PAGE_SIZE,
    totalItems: DEFAULT_TOTAL_ITEMS,
    totalPages: DEFAULT_TOTAL_PAGES,
  });
  const navigate = useNavigate();

  useEffect(() => {
    bookApi.getBooks({ pageNumber: DEFAULT_PAGE_NUMBER, pageSize: BOOKS_PAGE_SIZE, sortDescending: true }).then(setBooks);
  }, []);

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