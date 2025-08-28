import { useEffect, useState } from 'react';
import type { BookDto, BookFilterDto, PagedResponseDto, TagDto } from '../types';
import { bookApi, tagApi } from '../services/api';
import BookCard from '../components/BookCard';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import Button from '../components/Button';
import { BOOKS_PAGE_SIZE, TAGS_PAGE_SIZE, DEFAULT_PAGE_NUMBER, DEFAULT_TOTAL_ITEMS, DEFAULT_TOTAL_PAGES } from '../constants';

export default function BookListPage() {
  const { t } = useTranslation();
  const [filter, setFilter] = useState<BookFilterDto>({
    pageNumber: DEFAULT_PAGE_NUMBER,
    pageSize: BOOKS_PAGE_SIZE,
    sortDescending: false,
    sortBy: 'title',
  });
  const [books, setBooks] = useState<PagedResponseDto<BookDto>>({
    items: [],
    pageNumber: DEFAULT_PAGE_NUMBER,
    pageSize: BOOKS_PAGE_SIZE,
    totalItems: DEFAULT_TOTAL_ITEMS,
    totalPages: DEFAULT_TOTAL_PAGES,
  });
  const [tags, setTags] = useState<TagDto[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    Promise.all([
      bookApi.getBooks(filter).then(setBooks),
      tagApi.getTags({ pageNumber: DEFAULT_PAGE_NUMBER, pageSize: TAGS_PAGE_SIZE }).then((res) => setTags(res.items)),
    ]);
  }, [filter]);

  const updateSearchQuery = (searchQuery: string) => {
    setFilter({ ...filter, searchQuery, pageNumber: DEFAULT_PAGE_NUMBER });
  };

  const updateTagFilter = (tagIds: string[]) => {
    setFilter({ ...filter, tagIds, pageNumber: DEFAULT_PAGE_NUMBER });
  };

  const updateSort = (sortBy: 'title' | 'createdAt', sortDescending: boolean) => {
    setFilter({ ...filter, sortBy, sortDescending, pageNumber: DEFAULT_PAGE_NUMBER });
  };

  const updatePage = (pageNumber: number) => {
    setFilter({ ...filter, pageNumber });
  };

  return (
    <div className="container mx-auto p-4 sm:p-6">
      <h1 className="text-2xl sm:text-3xl font-bold text-gray-800 mb-6">{t('Books')}</h1>
      <div className="flex flex-col md:flex-row gap-4">
        <aside className="w-full md:w-1/4 flex flex-col gap-4 bg-white p-4 rounded-lg shadow-md">
          <input
            type="text"
            placeholder={t('SearchBooks')}
            onChange={(e) => updateSearchQuery(e.target.value)}
            className="w-full p-2 border rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          />
          <select
            multiple
            onChange={(e) => updateTagFilter(Array.from(e.target.selectedOptions, (option) => option.value))}
            className="w-full p-2 border rounded-lg text-sm"
          >
            {tags.map((tag) => (
              <option key={tag.id} value={tag.id}>
                {tag.tagName}
              </option>
            ))}
          </select>
          <div className="flex flex-col gap-2">
            <select
              onChange={(e) => updateSort(e.target.value as 'title' | 'createdAt', filter.sortDescending)}
              className="w-full p-2 border rounded-lg text-sm"
            >
              <option value="title">{t('SortByTitle')}</option>
              <option value="createdAt">{t('SortByCreatedAt')}</option>
            </select>
            <Button
              variant="primary"
              onClick={() => updateSort(filter.sortBy as 'title' | 'createdAt', !filter.sortDescending)}
              className="px-3 py-1.5 text-sm"
            >
              {filter.sortDescending ? t('SortAsc') : t('SortDesc')}
            </Button>
          </div>
        </aside>
        <div className="w-full md:w-3/4">
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-6">
            {books.items.length === 0 ? (
              <p className="text-gray-600 text-center col-span-full">{t('NoBooksAvailable')}</p>
            ) : (
              books.items.map((book) => (
                <BookCard key={book.id} book={book} onClick={() => navigate(`/books/${book.id}`)} />
              ))
            )}
          </div>
          <div className="mt-6 flex flex-col items-center gap-2">
            <span className="text-gray-700">
              {t('Page')} {books.pageNumber} {t('Of')} {books.totalPages}
            </span>
            <div className="flex justify-center gap-4">
              <Button
                variant="primary"
                disabled={books.pageNumber === DEFAULT_PAGE_NUMBER}
                onClick={() => updatePage(books.pageNumber - 1)}
                className="px-3 py-1.5 text-sm"
              >
                {t('Previous')}
              </Button>
              <Button
                variant="primary"
                disabled={books.pageNumber === books.totalPages}
                onClick={() => updatePage(books.pageNumber + 1)}
                className="px-3 py-1.5 text-sm"
              >
                {t('Next')}
              </Button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}