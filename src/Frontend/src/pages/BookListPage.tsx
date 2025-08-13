import { useEffect, useState } from 'react';
import type { BookDto, BookFilterDto, PagedResponseDto, TagDto } from '../types';
import { bookApi, tagApi } from '../services/api';
import BookCard from '../components/BookCard';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { toast } from 'react-toastify';
import Button from '../components/Button';
import { BOOKS_PAGE_SIZE, TAGS_PAGE_SIZE } from '../constants';

export default function BookListPage() {
  const { t } = useTranslation();
  const [filter, setFilter] = useState<BookFilterDto>({
    pageNumber: 1,
    pageSize: BOOKS_PAGE_SIZE,
    sortDescending: false,
    sortBy: 'title',
  });
  const [books, setBooks] = useState<PagedResponseDto<BookDto>>({
    items: [],
    pageNumber: 1,
    pageSize: BOOKS_PAGE_SIZE,
    totalItems: 0,
    totalPages: 0,
  });
  const [tags, setTags] = useState<TagDto[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    bookApi.getBooks(filter).then(setBooks).catch(() => toast.error(t('FailedToLoadBooks')));
    tagApi.getTags({ pageNumber: 1, pageSize: TAGS_PAGE_SIZE }).then((res) => setTags(res.items));
  }, [filter, t]);

  const handleSearch = (searchQuery: string) => {
    setFilter({ ...filter, searchQuery, pageNumber: 1 });
  };

  const handleTagFilter = (tagIds: string[]) => {
    setFilter({ ...filter, tagIds, pageNumber: 1 });
  };

  const handleSort = (sortBy: 'title' | 'createdAt', sortDescending: boolean) => {
    setFilter({ ...filter, sortBy, sortDescending, pageNumber: 1 });
  };

  const handlePageChange = (pageNumber: number) => {
    setFilter({ ...filter, pageNumber });
  };

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">{t('Books')}</h1>
      <div className="mb-6 flex flex-col sm:flex-row gap-4 bg-white p-4 rounded-lg shadow-md">
        <input
          type="text"
          placeholder={t('SearchBooks')}
          onChange={(e) => handleSearch(e.target.value)}
          className="w-full sm:w-1/4 p-2 border rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        />
        <select
          multiple
          onChange={(e) => handleTagFilter(Array.from(e.target.selectedOptions, (option) => option.value))}
          className="w-full sm:w-1/4 p-2 border rounded-lg text-sm"
        >
          {tags.map((tag) => (
            <option key={tag.id} value={tag.id}>
              {tag.tagName}
            </option>
          ))}
        </select>
        <div className="flex gap-2">
          <select
            onChange={(e) => handleSort(e.target.value as 'title' | 'createdAt', filter.sortDescending)}
            className="p-2 border rounded-lg text-sm"
          >
            <option value="title">{t('SortByTitle')}</option>
            <option value="createdAt">{t('SortByCreatedAt')}</option>
          </select>
          <Button
            variant="primary"
            onClick={() => handleSort(filter.sortBy as 'title' | 'createdAt', !filter.sortDescending)}
            className="px-3 py-1.5 text-sm"
          >
            {filter.sortDescending ? t('SortAsc') : t('SortDesc')}
          </Button>
        </div>
      </div>
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        {books.items.map((book) => (
          <BookCard key={book.id} book={book} onClick={() => navigate(`/books/${book.id}`)} />
        ))}
      </div>
      <div className="mt-6 flex justify-center gap-4">
        <Button
          variant="primary"
          disabled={books.pageNumber === 1}
          onClick={() => handlePageChange(books.pageNumber - 1)}
          className="px-3 py-1.5 text-sm"
        >
          {t('Previous')}
        </Button>
        <span className="text-gray-700">
          {t('Page')} {books.pageNumber} {t('Of')} {books.totalPages}
        </span>
        <Button
          variant="primary"
          disabled={books.pageNumber === books.totalPages}
          onClick={() => handlePageChange(books.pageNumber + 1)}
          className="px-3 py-1.5 text-sm"
        >
          {t('Next')}
        </Button>
      </div>
    </div>
  );
}