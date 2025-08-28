import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './i18n';
import './index.css';
import App from './App';
import HomePage from './pages/HomePage';
import BookListPage from './pages/BookListPage';
import BookDetailsPage from './pages/BookDetailsPage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import AdminPanel from './pages/AdminPanel';
import CreateBookPage from './pages/CreateBookPage';
import EditBookPage from './pages/EditBookPage';
import UserProfilePage from './pages/UserProfilePage';
import PrivateRoute from './components/PrivateRoute.tsx'; 
import { useAuthStore } from './store/authStore';
import { useEffect } from 'react';

function Root() {
  const { initialize } = useAuthStore();

  useEffect(() => {
    initialize();
  }, [initialize]);

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<App />}>
          <Route index element={<HomePage />} />
          <Route path="books" element={<BookListPage />} />
          <Route path="books/:id" element={<BookDetailsPage />} />
          <Route path="login" element={<LoginPage />} />
          <Route path="register" element={<RegisterPage />} />
          <Route element={<PrivateRoute />}>
            <Route path="profile" element={<UserProfilePage />} />
            <Route path="books/create" element={<CreateBookPage />} />
            <Route path="books/:id/edit" element={<EditBookPage />} />
          </Route>
          <Route element={<PrivateRoute adminOnly />}>
            <Route path="admin" element={<AdminPanel />} />
          </Route>
        </Route>
      </Routes>
      <ToastContainer />
    </BrowserRouter>
  );
}

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Root />
  </StrictMode>,
);