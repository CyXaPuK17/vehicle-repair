import { Navigate } from 'react-router-dom';
import { useAuthStore } from '../../store/authStore';

export default function RoleRedirect() {
  const role = useAuthStore(s => s.role);
  if (role === 'Customer') return <Navigate to="/active-repairs" replace />;
  if (role === 'Executor') return <Navigate to="/queue" replace />;
  return <Navigate to="/dashboard" replace />;
}
