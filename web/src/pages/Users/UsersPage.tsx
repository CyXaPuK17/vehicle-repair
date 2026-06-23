import { useEffect, useState } from 'react';
import { Table, Card, Button, Modal, Form, Input, Select, Tag, Typography, message, Divider } from 'antd';
import { PlusOutlined, KeyOutlined, EditOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { getUsers, createUser, updateUser, changePassword, setUserActive } from '../../api/users';
import { getCustomers } from '../../api/customers';
import { getExecutors } from '../../api/executors';
import type { UserDto, CustomerDto, ExecutorDto, UserRole } from '../../types';
import dayjs from 'dayjs';
import { PAGINATION } from '../../utils/pagination';

const ROLE_LABELS: Record<string, string> = {
  ManagementCompany: 'УК',
  Customer: 'Заказчик',
  Executor: 'Исполнитель',
};

const ROLE_COLORS: Record<string, string> = {
  ManagementCompany: 'blue',
  Customer: 'orange',
  Executor: 'green',
};

export default function UsersPage() {
  const [users, setUsers] = useState<UserDto[]>([]);
  const [customers, setCustomers] = useState<CustomerDto[]>([]);
  const [executors, setExecutors] = useState<ExecutorDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [createOpen, setCreateOpen] = useState(false);
  const [editOpen, setEditOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<UserDto | null>(null);
  const [pwdOpen, setPwdOpen] = useState(false);
  const [createForm] = Form.useForm();
  const [editForm] = Form.useForm();
  const [pwdForm] = Form.useForm();
  const [selectedRole, setSelectedRole] = useState<UserRole | null>(null);
  const [editRole, setEditRole] = useState<UserRole | null>(null);

  const load = async () => {
    try {
      const [uRes, cRes, eRes] = await Promise.all([getUsers(), getCustomers(), getExecutors()]);
      setUsers(uRes.data.data!);
      setCustomers(cRes.data.data!.filter(c => c.isActive));
      setExecutors(eRes.data.data!.filter(e => e.isActive));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const handleCreate = async () => {
    const values = await createForm.validateFields();
    try {
      await createUser(values);
      message.success('Пользователь создан');
      setCreateOpen(false);
      createForm.resetFields();
      setSelectedRole(null);
      load();
    } catch {
      message.error('Ошибка при создании');
    }
  };

  const openEdit = (user: UserDto) => {
    setEditingUser(user);
    setEditRole(user.role as UserRole);
    editForm.setFieldsValue({
      login: user.login,
      role: user.role,
      customerId: user.customerId ?? undefined,
      executorId: user.executorId ?? undefined,
    });
    setEditOpen(true);
  };

  const handleEdit = async () => {
    const values = await editForm.validateFields();
    try {
      await updateUser(editingUser!.id, values);
      message.success('Пользователь обновлён');
      setEditOpen(false);
      load();
    } catch {
      message.error('Ошибка при обновлении');
    }
  };

  const handleToggleActive = async (user: UserDto) => {
    try {
      await setUserActive(user.id, !user.isActive);
      message.success(user.isActive ? 'Пользователь деактивирован' : 'Пользователь активирован');
      load();
    } catch {
      message.error('Ошибка при изменении статуса');
    }
  };

  const handleChangePwd = async () => {
    const { currentPassword, newPassword } = await pwdForm.validateFields();
    try {
      await changePassword(currentPassword, newPassword);
      message.success('Пароль изменён');
      setPwdOpen(false);
      pwdForm.resetFields();
    } catch {
      message.error('Ошибка при смене пароля');
    }
  };

  const columns: ColumnsType<UserDto> = [
    { title: 'Логин', dataIndex: 'login', sorter: (a, b) => a.login.localeCompare(b.login) },
    {
      title: 'Роль', dataIndex: 'role',
      sorter: (a, b) => (ROLE_LABELS[a.role] ?? a.role).localeCompare(ROLE_LABELS[b.role] ?? b.role),
      render: (v: string) => <Tag color={ROLE_COLORS[v]}>{ROLE_LABELS[v] ?? v}</Tag>,
    },
    {
      title: 'Привязан к', dataIndex: 'linkedEntityName',
      sorter: (a, b) => (a.linkedEntityName ?? '').localeCompare(b.linkedEntityName ?? ''),
    },
    {
      title: 'Последний вход', dataIndex: 'lastLoginAt',
      sorter: (a, b) => new Date(a.lastLoginAt ?? 0).getTime() - new Date(b.lastLoginAt ?? 0).getTime(),
      render: (v?: string) => v ? dayjs(v).format('DD.MM.YYYY HH:mm') : '—',
    },
    {
      title: 'Статус', dataIndex: 'isActive',
      sorter: (a, b) => Number(a.isActive) - Number(b.isActive),
      render: (v: boolean, record: UserDto) => (
        <Button
          size="small"
          type={v ? 'default' : 'primary'}
          danger={v}
          onClick={() => handleToggleActive(record)}
        >
          {v ? 'Деактивировать' : 'Активировать'}
        </Button>
      ),
    },
    {
      title: '',
      key: 'actions',
      width: 48,
      render: (_: unknown, record: UserDto) => (
        <Button size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
      ),
    },
  ];

  return (
    <Card
      title={<Typography.Title level={4} style={{ margin: 0 }}>Пользователи</Typography.Title>}
      extra={
        <Button.Group>
          <Button icon={<KeyOutlined />} onClick={() => { pwdForm.resetFields(); setPwdOpen(true); }}>
            Сменить пароль
          </Button>
          <Button type="primary" icon={<PlusOutlined />} onClick={() => { createForm.resetFields(); setSelectedRole(null); setCreateOpen(true); }}>
            Добавить
          </Button>
        </Button.Group>
      }
    >
      <Table
        dataSource={users}
        columns={columns}
        rowKey="id"
        loading={loading}
        size="small"
        pagination={PAGINATION}
        showSorterTooltip={false}
      />

      <Modal title="Редактировать пользователя" open={editOpen} onOk={handleEdit} onCancel={() => setEditOpen(false)} okText="Сохранить">
        <Form form={editForm} layout="vertical">
          <Form.Item name="login" label="Логин / ИНН" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="role" label="Роль" rules={[{ required: true }]}>
            <Select
              options={[
                { value: 'ManagementCompany', label: 'Управляющая компания' },
                { value: 'Customer', label: 'Заказчик' },
                { value: 'Executor', label: 'Исполнитель' },
              ]}
              onChange={(v) => { setEditRole(v as UserRole); editForm.setFieldsValue({ customerId: undefined, executorId: undefined }); }}
            />
          </Form.Item>
          {editRole === 'Customer' && (
            <Form.Item name="customerId" label="Заказчик" rules={[{ required: true }]}>
              <Select
                showSearch
                options={customers.map(c => ({ value: c.id, label: `${c.name} (${c.inn})` }))}
                filterOption={(input, opt) => (opt?.label as string ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item>
          )}
          {editRole === 'Executor' && (
            <Form.Item name="executorId" label="Исполнитель" rules={[{ required: true }]}>
              <Select
                showSearch
                options={executors.map(e => ({ value: e.id, label: `${e.name} (${e.inn})` }))}
                filterOption={(input, opt) => (opt?.label as string ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item>
          )}
        </Form>
      </Modal>

      <Modal title="Новый пользователь" open={createOpen} onOk={handleCreate} onCancel={() => setCreateOpen(false)} okText="Создать">
        <Form form={createForm} layout="vertical">
          <Form.Item name="login" label="Логин / ИНН" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="password" label="Пароль" rules={[{ required: true, min: 6 }]}>
            <Input.Password />
          </Form.Item>
          <Form.Item name="role" label="Роль" rules={[{ required: true }]}>
            <Select
              options={[
                { value: 'ManagementCompany', label: 'Управляющая компания' },
                { value: 'Customer', label: 'Заказчик' },
                { value: 'Executor', label: 'Исполнитель' },
              ]}
              onChange={(v) => { setSelectedRole(v as UserRole); createForm.setFieldsValue({ customerId: undefined, executorId: undefined }); }}
            />
          </Form.Item>
          {selectedRole === 'Customer' && (
            <Form.Item name="customerId" label="Заказчик" rules={[{ required: true }]}>
              <Select
                showSearch
                options={customers.map(c => ({ value: c.id, label: `${c.name} (${c.inn})` }))}
                filterOption={(input, opt) => (opt?.label as string ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item>
          )}
          {selectedRole === 'Executor' && (
            <Form.Item name="executorId" label="Исполнитель" rules={[{ required: true }]}>
              <Select
                showSearch
                options={executors.map(e => ({ value: e.id, label: `${e.name} (${e.inn})` }))}
                filterOption={(input, opt) => (opt?.label as string ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item>
          )}
        </Form>
      </Modal>

      <Modal title="Смена пароля" open={pwdOpen} onOk={handleChangePwd} onCancel={() => setPwdOpen(false)} okText="Изменить">
        <Form form={pwdForm} layout="vertical">
          <Form.Item name="currentPassword" label="Текущий пароль" rules={[{ required: true }]}>
            <Input.Password />
          </Form.Item>
          <Divider />
          <Form.Item name="newPassword" label="Новый пароль" rules={[{ required: true, min: 6 }]}>
            <Input.Password />
          </Form.Item>
          <Form.Item
            name="confirmPassword"
            label="Повторите пароль"
            dependencies={['newPassword']}
            rules={[
              { required: true },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue('newPassword') === value) return Promise.resolve();
                  return Promise.reject(new Error('Пароли не совпадают'));
                },
              }),
            ]}
          >
            <Input.Password />
          </Form.Item>
        </Form>
      </Modal>
    </Card>
  );
}
