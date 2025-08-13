import { Component, type ReactNode } from 'react';
import { useTranslation } from 'react-i18next';

class ErrorBoundary extends Component<{ children: ReactNode }, { hasError: boolean }> {
  state = { hasError: false };

  static getDerivedStateFromError() {
    return { hasError: true };
  }

  render() {
    if (this.state.hasError) {
      const { t } = useTranslation();
      return <div className="text-red-500 text-center p-4">{t('SomethingWentWrong')}</div>;
    }
    return this.props.children;
  }
}

export default ErrorBoundary;