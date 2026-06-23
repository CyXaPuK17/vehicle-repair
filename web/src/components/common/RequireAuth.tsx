import { Navigate } from 'react-router-dom';
import { Spin } from 'antd';
import { useAuthStore } from '../../store/authStore';

export function RequireAuth({ children, roles }: { children: React.ReactNode; roles?: string[] }) {
  const initializing = useAuthStore(s => s.initializing);
  const isAuthenticated = useAuthStore(s => s.isAuthenticated);
  const role = useAuthStore(s => s.role);

  if (initializing) return <Spin fullscreen />;
  if (!isAuthenticated()) return <Navigate to="/login" replace />;
  if (roles && role && !roles.includes(role)) return <Navigate to="/403" replace />;
  return <>{children}</>;
}
