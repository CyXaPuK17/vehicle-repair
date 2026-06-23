import { useState } from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { Layout, Menu, Button, Typography, Space, Dropdown, Modal, Form, Input, message } from 'antd';
import {
  FileTextOutlined, TeamOutlined, CarOutlined, ToolOutlined,
  UserOutlined, LogoutOutlined, BarChartOutlined, DashboardOutlined,
  UnorderedListOutlined, FileSearchOutlined, KeyOutlined, DownOutlined,
} from '@ant-design/icons';
import { useAuthStore } from '../../store/authStore';
import { logout } from '../../api/auth';
import { changePassword } from '../../api/users';

const { Sider, Header, Content } = Layout;

const roleLabel: Record<string, string> = {
  ManagementCompany: 'УК',
  Executor: 'Исполнитель',
  Customer: 'Заказчик',
};

export default function AppLayout() {
  const { role, logout: clearAuth } = useAuthStore();
  const navigate = useNavigate();
  const location = useLocation();

  const isUK       = role === 'ManagementCompany';
  const isExecutor = role === 'Executor';
  const isCustomer = role === 'Customer';

  const [pwdOpen, setPwdOpen]       = useState(false);
  const [pwdLoading, setPwdLoading] = useState(false);
  const [pwdForm] = Form.useForm();

  const handleLogout = async () => {
    await logout().catch(() => {});
    clearAuth();
    navigate('/login');
  };

  const handleChangePwd = async () => {
    const { currentPassword, newPassword, confirm } = await pwdForm.validateFields();
    if (newPassword !== confirm) { message.error('Пароли не совпадают'); return; }
    setPwdLoading(true);
    try {
      await changePassword(currentPassword, newPassword);
      message.success('Пароль изменён');
      setPwdOpen(false);
      pwdForm.resetFields();
    } catch {
      message.error('Неверный текущий пароль');
    } finally {
      setPwdLoading(false);
    }
  };

  const menuItems = [
    ...(isUK ? [{ key: '/dashboard', icon: <DashboardOutlined />, label: 'Главная' }] : []),
    ...(isUK ? [{ key: '/repairs', icon: <FileSearchOutlined />, label: 'Ремонты' }] : []),
    ...(isExecutor ? [{ key: '/queue', icon: <UnorderedListOutlined />, label: 'Мои задания' }] : []),
    ...(isCustomer ? [{ key: '/active-repairs', icon: <CarOutlined />, label: 'Текущие ремонты' }] : []),
    ...(isUK || isExecutor ? [{ key: '/reports/by-repairs', icon: <FileTextOutlined />, label: 'Отчёт по ремонтам' }] : []),
    ...(isUK || isCustomer ? [{ key: '/reports/by-vehicle', icon: <CarOutlined />, label: 'Отчёт по ТС' }] : []),
    ...(isUK ? [
      { key: '/reports/by-customer', icon: <BarChartOutlined />, label: 'Отчёт по заказчику' },
      { key: '/reports/by-executor', icon: <BarChartOutlined />, label: 'Отчёт по исполнителю' },
      { type: 'divider' as const },
      { key: '/customers',    icon: <TeamOutlined />,   label: 'Заказчики' },
      { key: '/executors',    icon: <TeamOutlined />,   label: 'Исполнители' },
      { key: '/vehicles',     icon: <CarOutlined />,    label: 'ТС' },
      { key: '/repair-types', icon: <ToolOutlined />,   label: 'Виды ремонта' },
      { key: '/users',        icon: <UserOutlined />,   label: 'Пользователи' },
    ] : []),
  ];

  const userMenu = {
    items: [
      { key: 'pwd', icon: <KeyOutlined />, label: 'Сменить пароль' },
      { type: 'divider' as const },
      { key: 'logout', icon: <LogoutOutlined />, label: 'Выйти', danger: true },
    ],
    onClick: ({ key }: { key: string }) => {
      if (key === 'pwd') setPwdOpen(true);
      if (key === 'logout') handleLogout();
    },
  };

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider width={220} theme="light" style={{ borderRight: '1px solid #f0f0f0' }}>
        <div style={{ padding: '16px', fontWeight: 700, fontSize: 15, borderBottom: '1px solid #f0f0f0' }}>
          Учёт ремонтов ТС
        </div>
        <Menu
          mode="inline"
          selectedKeys={[location.pathname]}
          items={menuItems}
          onClick={({ key }) => navigate(key)}
          style={{ border: 'none' }}
        />
      </Sider>
      <Layout>
        <Header style={{ background: '#fff', borderBottom: '1px solid #f0f0f0', padding: '0 24px', display: 'flex', justifyContent: 'flex-end', alignItems: 'center' }}>
          <Dropdown menu={userMenu} trigger={['click']}>
            <Space style={{ cursor: 'pointer' }}>
              <Typography.Text type="secondary">{role ? roleLabel[role] : ''}</Typography.Text>
              <DownOutlined style={{ fontSize: 10, color: '#999' }} />
            </Space>
          </Dropdown>
        </Header>
        <Content style={{ padding: 24, background: '#f5f5f5' }}>
          <Outlet />
        </Content>
      </Layout>

      <Modal
        title="Сменить пароль"
        open={pwdOpen}
        onOk={handleChangePwd}
        onCancel={() => { setPwdOpen(false); pwdForm.resetFields(); }}
        okText="Сохранить"
        confirmLoading={pwdLoading}
      >
        <Form form={pwdForm} layout="vertical" style={{ marginTop: 8 }}>
          <Form.Item name="currentPassword" label="Текущий пароль" rules={[{ required: true }]}>
            <Input.Password />
          </Form.Item>
          <Form.Item name="newPassword" label="Новый пароль" rules={[{ required: true, min: 6 }]}>
            <Input.Password />
          </Form.Item>
          <Form.Item name="confirm" label="Подтверждение" rules={[{ required: true }]}>
            <Input.Password />
          </Form.Item>
        </Form>
      </Modal>
    </Layout>
  );
}
