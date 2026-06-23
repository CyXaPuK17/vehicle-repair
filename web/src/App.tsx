import { useEffect } from 'react';
import { RouterProvider } from 'react-router-dom';
import { ConfigProvider } from 'antd';
import ruRU from 'antd/locale/ru_RU';
import axios from 'axios';
import { router } from './router';
import { useAuthStore } from './store/authStore';
import type { UserRole } from './types';

export default function App() {
  const setAuth = useAuthStore(s => s.setAuth);
  const setInitializing = useAuthStore(s => s.setInitializing);

  useEffect(() => {
    const controller = new AbortController();

    axios
      .post<{ data: { accessToken: string; role: UserRole; userId: string } }>(
        '/api/v1/auth/refresh',
        {},
        { withCredentials: true, signal: controller.signal }
      )
      .then(res => {
        const { accessToken, role, userId } = res.data.data;
        setAuth(accessToken, role, userId);
      })
      .catch(err => {
        if (axios.isCancel(err)) return;
        // Нет валидной куки refresh-токена — пользователь должен войти
      })
      .finally(() => {
        if (!controller.signal.aborted) setInitializing(false);
      });

    return () => controller.abort();
  }, []);

  return (
    <ConfigProvider locale={ruRU}>
      <RouterProvider router={router} />
    </ConfigProvider>
  );
}
