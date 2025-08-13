import { useTranslation } from 'react-i18next';
import { toast } from 'react-toastify';
import { bookApi } from '../services/api';
import BookForm from '../components/BookForm';
import type { BookCreateDto, BookUpdateDto } from '../types';
import { useNavigate } from 'react-router-dom';

export default function CreateBookPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();

  const handleSubmit = async (data: BookCreateDto) => {
    try {
      const newBook = await bookApi.createBook({ ...data, tagIds: data.tags });
      if (data.coverImage) {
        await bookApi.uploadCover(newBook.id, data.coverImage[0]);
      }
      toast.success(t('BookCreated'));
      navigate('/books');
    } catch (error: any) {
      toast.error(t(error.response?.data?.errors?.[0] || 'UnknownError'));
    }
  };

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">{t('CreateBook')}</h1>
      <BookForm onSubmit={handleSubmit as (data: BookCreateDto | BookUpdateDto) => void} />
    </div>
  );
}