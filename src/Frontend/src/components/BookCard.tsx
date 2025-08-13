import type { BookDto } from '../types';
import { useTranslation } from 'react-i18next';
import { BOOK_CARD_IMAGE_HEIGHT } from '../constants';

interface BookCardProps {
  book: BookDto;
  onClick?: () => void;
}

export default function BookCard({ book, onClick }: BookCardProps) {
  const { t } = useTranslation();

  return (
    <div
      className="p-6 border rounded-lg shadow-lg hover:shadow-xl transition-shadow cursor-pointer bg-white"
      onClick={onClick}
    >
      {book.coverImageUrl ? (
        <img
          src={book.coverImageUrl}
          alt={book.title}
          className={`w-full h-${BOOK_CARD_IMAGE_HEIGHT} object-cover rounded-lg mb-4`}
        />
      ) : (
        <div
          className={`w-full h-${BOOK_CARD_IMAGE_HEIGHT} bg-gray-200 flex items-center justify-center rounded-lg mb-4`}
        >
          <span className="text-gray-500">{t('NoImage')}</span>
        </div>
      )}
      <h2 className="text-xl font-bold text-blue-800 mb-2">{book.title}</h2>
      <p className="text-gray-700 mb-2">
        <strong>{t('Author')}:</strong> {book.author}
      </p>
      {book.description && (
        <p className="text-gray-600 line-clamp-3 mb-3">{book.description}</p>
      )}
      <div className="flex flex-wrap gap-2">
        {book.tags.map((tag, index) => (
          <span
            key={index}
            className="inline-block bg-blue-100 text-blue-900 text-sm font-medium px-3 py-1 rounded-full"
          >
            {tag.tagName}
          </span>
        ))}
      </div>
    </div>
  );
}