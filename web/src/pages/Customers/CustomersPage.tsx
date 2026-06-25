import { useEffect, useState } from 'react';
import { Table, Card, Button, Modal, Form, Input, Switch, Typography, message, Space } from 'antd';
import { PlusOutlined, SearchOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { getCustomers, createCustomer, updateCustomer } from '../../api/customers';
import type { CustomerDto } from '../../types';
import StatusTag from '../../components/common/StatusTag';
import { PAGINATION } from '../../utils/pagination';

export default function CustomersPage() {
  const [customers, setCustomers] = useState<CustomerDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<CustomerDto | null>(null);
  const [form] = Form.useForm();
  const [search, setSearch] = useState('');
  const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);
  const [bulkLoading, setBulkLoading] = useState(false);

  const load = async () => {
    try {
      const res = await getCustomers();
      setCustomers(res.data.data!);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const openCreate = () => { setEditing(null); form.resetFields(); setModalOpen(true); };
  const openEdit = (c: CustomerDto) => { setEditing(c); form.setFieldsValue(c); setModalOpen(true); };

  const handleOk = async () => {
    const values = await form.validateFields();
    try {
      if (editing) {
        await updateCustomer(editing.id, values);
        message.success('Заказчик обновлён');
      } else {
        await createCustomer(values);
        message.success('Заказчик создан');
      }
      setModalOpen(false);
      load();
    } catch {
      message.error('Ошибка при сохранении');
    }
  };

  const handleBulkStatus = async (isActive: boolean) => {
    setBulkLoading(true);
    try {
      const selected = customers.filter(c => selectedRowKeys.includes(c.id));
      await Promise.all(selected.map(c => updateCustomer(c.id, {
        name: c.name, contactPerson: c.contactPerson, phone: c.phone, email: c.email, isActive,
      })));
      setSelectedRowKeys([]);
      message.success(`${selected.length} записей обновлено`);
      load();
    } catch {
      message.error('Ошибка при обновлении');
    } finally {
      setBulkLoading(false);
    }
  };

  const columns: ColumnsType<CustomerDto> = [
    { title: 'ИНН', dataIndex: 'inn', width: 130, sorter: (a, b) => a.inn.localeCompare(b.inn) },
    { title: 'Наименование', dataIndex: 'name', sorter: (a, b) => a.name.localeCompare(b.name) },
    { title: 'Контакт', dataIndex: 'contactPerson', sorter: (a, b) => (a.contactPerson ?? '').localeCompare(b.contactPerson ?? '') },
    { title: 'Телефон', dataIndex: 'phone', sorter: (a, b) => (a.phone ?? '').localeCompare(b.phone ?? '') },
    { title: 'Email', dataIndex: 'email', sorter: (a, b) => (a.email ?? '').localeCompare(b.email ?? '') },
    {
      title: 'Статус', dataIndex: 'isActive',
      sorter: (a, b) => Number(a.isActive) - Number(b.isActive),
      render: (v: boolean) => <StatusTag isActive={v} />,
    },
    {
      title: '', key: 'actions',
      render: (_, r) => <Button size="small" onClick={() => openEdit(r)}>Редактировать</Button>,
    },
  ];

  const q = search.toLowerCase();
  const filtered = q
    ? customers.filter(c => [c.inn, c.name, c.contactPerson, c.phone, c.email].some(v => v?.toLowerCase().includes(q)))
    : customers;

  const selectedItems = customers.filter(c => selectedRowKeys.includes(c.id));
  const bulkBar = selectedRowKeys.length > 0 && (
    <Space>
      <Typography.Text type="secondary">{selectedRowKeys.length} выбрано</Typography.Text>
      {selectedItems.some(c => !c.isActive) && <Button size="small" loading={bulkLoading} onClick={() => handleBulkStatus(true)}>Активировать</Button>}
      {selectedItems.some(c => c.isActive) && <Button size="small" danger loading={bulkLoading} onClick={() => handleBulkStatus(false)}>Деактивировать</Button>}
    </Space>
  );

  return (
    <Card
      title={<Typography.Title level={4} style={{ margin: 0 }}>Заказчики</Typography.Title>}
      extra={
        <Space>
          {bulkBar}
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>Добавить</Button>
        </Space>
      }
    >
      <Input
        prefix={<SearchOutlined />}
        placeholder="Поиск по ИНН, названию, контакту, телефону, email..."
        value={search}
        onChange={e => setSearch(e.target.value)}
        allowClear
        style={{ marginBottom: 12 }}
      />
      <Table
        dataSource={filtered}
        columns={columns}
        rowKey="id"
        loading={loading}
        size="small"
        pagination={PAGINATION}
        showSorterTooltip={false}
        rowSelection={{ selectedRowKeys, onChange: setSelectedRowKeys }}
      />
      <Modal
        title={editing ? 'Редактировать заказчика' : 'Новый заказчик'}
        open={modalOpen}
        onOk={handleOk}
        onCancel={() => setModalOpen(false)}
        okText="Сохранить"
      >
        <Form form={form} layout="vertical">
          {!editing && (
            <Form.Item name="inn" label="ИНН" rules={[{ required: true, pattern: /^\d{10}(\d{2})?$/, message: 'ИНН: 10 или 12 цифр' }]}>
              <Input />
            </Form.Item>
          )}
          <Form.Item name="name" label="Наименование" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="contactPerson" label="Контактное лицо"><Input /></Form.Item>
          <Form.Item name="phone" label="Телефон"><Input /></Form.Item>
          <Form.Item name="email" label="Email" rules={[{ type: 'email' }]}><Input /></Form.Item>
          {editing && (
            <Form.Item name="isActive" label="Активен" valuePropName="checked">
              <Switch />
            </Form.Item>
          )}
        </Form>
      </Modal>
    </Card>
  );
}
