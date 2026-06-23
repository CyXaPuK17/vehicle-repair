import { createBrowserRouter } from 'react-router-dom';
import { RequireAuth } from '../components/common/RequireAuth';
import AppLayout from '../components/Layout/AppLayout';
import LoginPage from '../pages/Login/LoginPage';
import ReportByCustomerPage from '../pages/Reports/ByCustomer/ReportByCustomerPage';
import ReportByExecutorPage from '../pages/Reports/ByExecutor/ReportByExecutorPage';
import ReportByRepairsPage from '../pages/Reports/ByRepairs/ReportByRepairsPage';
import ReportByVehiclePage from '../pages/Reports/ByVehicle/ReportByVehiclePage';
import CustomersPage from '../pages/Customers/CustomersPage';
import ExecutorsPage from '../pages/Executors/ExecutorsPage';
import VehiclesPage from '../pages/Vehicles/VehiclesPage';
import RepairTypesPage from '../pages/RepairTypes/RepairTypesPage';
import UsersPage from '../pages/Users/UsersPage';
import RoleRedirect from '../components/common/RoleRedirect';
import DashboardPage from '../pages/Dashboard/DashboardPage';
import RepairsPage from '../pages/Repairs/RepairsPage';
import ActiveRepairsPage from '../pages/ActiveRepairs/ActiveRepairsPage';
import QueuePage from '../pages/Queue/QueuePage';

export const router = createBrowserRouter([
  { path: '/login', element: <LoginPage /> },
  {
    path: '/',
    element: <RequireAuth><AppLayout /></RequireAuth>,
    children: [
      { index: true, element: <RoleRedirect /> },
      {
        path: 'dashboard',
        element: (
          <RequireAuth roles={['ManagementCompany']}>
            <DashboardPage />
          </RequireAuth>
        ),
      },
      {
        path: 'repairs',
        element: (
          <RequireAuth roles={['ManagementCompany']}>
            <RepairsPage />
          </RequireAuth>
        ),
      },
      {
        path: 'active-repairs',
        element: (
          <RequireAuth roles={['Customer']}>
            <ActiveRepairsPage />
          </RequireAuth>
        ),
      },
      {
        path: 'queue',
        element: (
          <RequireAuth roles={['Executor']}>
            <QueuePage />
          </RequireAuth>
        ),
      },
      {
        path: 'reports/by-customer',
        element: (
          <RequireAuth roles={['ManagementCompany']}>
            <ReportByCustomerPage />
          </RequireAuth>
        ),
      },
      {
        path: 'reports/by-executor',
        element: (
          <RequireAuth roles={['ManagementCompany']}>
            <ReportByExecutorPage />
          </RequireAuth>
        ),
      },
      {
        path: 'reports/by-repairs',
        element: (
          <RequireAuth roles={['ManagementCompany', 'Executor']}>
            <ReportByRepairsPage />
          </RequireAuth>
        ),
      },
      {
        path: 'reports/by-vehicle',
        element: (
          <RequireAuth roles={['ManagementCompany', 'Customer']}>
            <ReportByVehiclePage />
          </RequireAuth>
        ),
      },
      {
        path: 'customers',
        element: (
          <RequireAuth roles={['ManagementCompany']}>
            <CustomersPage />
          </RequireAuth>
        ),
      },
      {
        path: 'executors',
        element: (
          <RequireAuth roles={['ManagementCompany']}>
            <ExecutorsPage />
          </RequireAuth>
        ),
      },
      {
        path: 'vehicles',
        element: (
          <RequireAuth roles={['ManagementCompany']}>
            <VehiclesPage />
          </RequireAuth>
        ),
      },
      {
        path: 'repair-types',
        element: (
          <RequireAuth roles={['ManagementCompany']}>
            <RepairTypesPage />
          </RequireAuth>
        ),
      },
      {
        path: 'users',
        element: (
          <RequireAuth roles={['ManagementCompany']}>
            <UsersPage />
          </RequireAuth>
        ),
      },
      { path: '403', element: <div style={{ padding: 32 }}><h2>403 — Доступ запрещён</h2></div> },
    ],
  },
]);
