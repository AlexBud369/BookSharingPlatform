import { useTranslation } from 'react-i18next';
import { bookApi } from '../services/api';
import BookForm from '../components/BookForm';
import type { BookCreateDto, BookUpdateDto } from '../types';
import { useNavigate } from 'react-router-dom';

export default function CreateBookPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();

  const createBook = async (data: BookCreateDto) => {
    return await bookApi.createBook({ ...data, tagIds: data.tags });
  };

  const uploadCoverImage = async (bookId: string, coverImage?: FileList) => {
    if (coverImage && coverImage.length > 0) {
      await bookApi.uploadCover(bookId, coverImage[0]);
    }
  };

  const navigateToBooks = () => {
    navigate('/books');
  };

  const handleSubmit = async (data: BookCreateDto) => {
    const newBook = await createBook(data);
    await uploadCoverImage(newBook.id, data.coverImage);
    navigateToBooks();
  };

  return (
    <div className="container mx-auto p-4 sm:p-6">
      <h1 className="text-2xl sm:text-3xl font-bold text-gray-800 mb-6">{t('CreateBook')}</h1>
      <BookForm onSubmit={handleSubmit as (data: BookCreateDto | BookUpdateDto) => void} />
    </div>
  );
}