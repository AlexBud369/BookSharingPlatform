import { useForm, Controller } from 'react-hook-form';
import type { BookCreateDto, BookUpdateDto, TagDto, BookDtoBase } from '../types';
import { tagApi, bookApi } from '../services/api';
import { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import Select, { type MultiValue } from 'react-select';
import Button from './Button';
import { useAuthStore } from '../store/authStore';
import { MAX_TITLE_LENGTH, MAX_AUTHOR_LENGTH, MAX_DESCRIPTION_LENGTH, TAGS_PAGE_SIZE, DEFAULT_PAGE_NUMBER } from '../constants';

interface SelectOption {
  value: string;
  label: string;
}

const schema = yup.object({
  title: yup.string().required('TitleRequired').max(MAX_TITLE_LENGTH, 'TitleTooLong'),
  author: yup.string().required('AuthorRequired').max(MAX_AUTHOR_LENGTH, 'AuthorTooLong'),
  description: yup.string().max(MAX_DESCRIPTION_LENGTH, 'DescriptionTooLong').optional(),
  tags: yup.array().of(yup.string().required()).min(1, 'TagsRequired').required(),
  coverImage: yup.mixed<FileList>().optional(),
}).required();

interface BookFormProps {
  initialData?: BookUpdateDto;
  bookId?: string;
  onSubmit: (data: BookCreateDto | BookUpdateDto) => void;
}

export default function BookForm({ initialData, bookId, onSubmit }: BookFormProps) {
  const { t } = useTranslation();
  const { user } = useAuthStore();
  const isAdmin = user?.role === 'Admin';
  const { register, handleSubmit, control, formState: { errors } } = useForm<BookCreateDto>({
    resolver: yupResolver(schema),
    defaultValues: initialData
      ? {
          title: initialData.title || '',
          author: initialData.author || '',
          description: initialData.description || '',
          tags: initialData.tags || [],
          coverImage: undefined,
        }
      : {
          title: '',
          author: '',
          description: '',
          tags: [],
          coverImage: undefined,
        },
  });
  const [tags, setTags] = useState<TagDto[]>([]);

  useEffect(() => {
    tagApi.getTags({ pageNumber: DEFAULT_PAGE_NUMBER, pageSize: TAGS_PAGE_SIZE }).then((res) => setTags(res.items));
  }, []);

  const handleCreateTag = async (inputValue: string) => {
    if (!isAdmin) return;
    const newTag = await tagApi.createTag({ tagName: inputValue });
    setTags((prev) => [...prev, newTag]);
    return newTag.id;
  };

  const uploadCoverImage = async (bookId: string, coverImage?: FileList) => {
    if (bookId && coverImage && coverImage.length > 0) {
      await bookApi.uploadCover(bookId, coverImage[0]);
    }
  };

  const onSubmitForm = async (data: BookCreateDto) => {
    const submitData: BookDtoBase = {
      title: data.title,
      author: data.author,
      description: data.description,
      tags: data.tags,
      coverImage: data.coverImage,
    };
    await uploadCoverImage(bookId!, data.coverImage);
    onSubmit(submitData);
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmitForm)}
      className="space-y-4 max-w-md sm:max-w-lg md:max-w-2xl mx-auto p-4 sm:p-6 bg-white rounded-lg shadow-md"
    >
      <div>
        <label className="block text-sm font-semibold text-gray-800">{t('Title')}</label>
        <input
          {...register('title')}
          placeholder={t('Title')}
          className="w-full p-2 sm:p-3 border rounded-lg text-sm sm:text-base focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        />
        {errors.title && <p className="text-red-600 mt-1">{t(errors.title.message!)}</p>}
      </div>
      <div>
        <label className="block text-sm font-semibold text-gray-800">{t('Author')}</label>
        <input
          {...register('author')}
          placeholder={t('Author')}
          className="w-full p-2 sm:p-3 border rounded-lg text-sm sm:text-base focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        />
        {errors.author && <p className="text-red-600 mt-1">{t(errors.author.message!)}</p>}
      </div>
      <div>
        <label className="block text-sm font-semibold text-gray-800">{t('Description')}</label>
        <textarea
          {...register('description')}
          placeholder={t('Description')}
          className="w-full p-2 sm:p-3 border rounded-lg text-sm sm:text-base focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        />
        {errors.description && <p className="text-red-600 mt-1">{t(errors.description.message!)}</p>}
      </div>
      <div>
        <label className="block text-sm font-semibold text-gray-800">{t('Tags')}</label>
        <Controller
          name="tags"
          control={control}
          render={({ field }) => (
            <Select
              isMulti
              options={tags.map((tag) => ({ value: tag.id, label: tag.tagName }))}
              value={field.value.map((id) => ({
                value: id,
                label: tags.find((tag) => tag.id === id)?.tagName || id,
              }))}
              onChange={(selected: MultiValue<SelectOption>) =>
                field.onChange(selected.map((option: SelectOption) => option.value))
              }
              onCreateOption={isAdmin ? handleCreateTag : undefined}
              className="w-full text-sm sm:text-base"
              placeholder={t('Tags')}
              isDisabled={!isAdmin && tags.length === 0}
              formatCreateLabel={(inputValue: string) => `${t('CreateTag')} "${inputValue}"`}
            />
          )}
        />
        {errors.tags && <p className="text-red-600 mt-1">{t(errors.tags.message!)}</p>}
      </div>
      <div>
        <label className="block text-sm font-semibold text-gray-800">{t('CoverImage')}</label>
        <input
          type="file"
          accept="image/*"
          {...register('coverImage')}
          className="w-full p-2 sm:p-3 border rounded-lg text-sm sm:text-base"
        />
        {errors.coverImage && <p className="text-red-600 mt-1">{t(errors.coverImage.message!)}</p>}
      </div>
      <Button type="submit" variant="primary" className="px-3 py-1.5 text-sm sm:text-base">
        {bookId ? t('EditBook') : t('CreateBook')}
      </Button>
    </form>
  );
}