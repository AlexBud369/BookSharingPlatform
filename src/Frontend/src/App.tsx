import { Outlet } from 'react-router-dom';
import Header from './components/Header';
import ErrorBoundary from './components/ErrorBoundary';
import { useTranslation } from 'react-i18next';
import { useLoadingStore } from './store/loadingStore';

export default function App() {
  const { t } = useTranslation();
  const { isLoading } = useLoadingStore();

  return (
    <div className="min-h-screen bg-gray-50">
      <Header />
      {isLoading && (
        <div className="fixed top-0 left-0 w-full h-full bg-black bg-opacity-50 flex items-center justify-center">
          <div className="text-gray-100 text-xl">{t('Loading')}</div>
        </div>
      )}
      <ErrorBoundary>
        <main className="container mx-auto p-6 bg-white rounded-lg shadow-md">
          <Outlet />
        </main>
      </ErrorBoundary>
    </div>
  );
}