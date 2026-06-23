import { useEffect, useState } from 'react';
import { Table, Card, Button, Modal, Form, Input, Switch, Typography, message, Space } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { getExecutors, createExecutor, updateExecutor } from '../../api/executors';
import type { ExecutorDto } from '../../types';
import StatusTag from '../../components/common/StatusTag';
import { PAGINATION } from '../../utils/pagination';

export default function ExecutorsPage() {
  const [executors, setExecutors] = useState<ExecutorDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<ExecutorDto | null>(null);
  const [form] = Form.useForm();
  const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);
  const [bulkLoading, setBulkLoading] = useState(false);

  const load = async () => {
    try {
      const res = await getExecutors();
      setExecutors(res.data.data!);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const openCreate = () => { setEditing(null); form.resetFields(); setModalOpen(true); };
  const openEdit = (e: ExecutorDto) => {
    setEditing(e);
    form.setFieldsValue({ name: e.name, address: e.address, phone: e.phone, email: e.email, isActive: e.isActive });
    setModalOpen(true);
  };

  const handleOk = async () => {
    const values = await form.validateFields();
    try {
      if (editing) {
        await updateExecutor(editing.id, values);
        message.success('Исполнитель обновлён');
      } else {
        await createExecutor(values);
        message.success('Исполнитель создан');
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
      const selected = executors.filter(e => selectedRowKeys.includes(e.id));
      await Promise.all(selected.map(e => updateExecutor(e.id, {
        name: e.name, address: e.address, phone: e.phone, email: e.email, isActive,
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

  const columns: ColumnsType<ExecutorDto> = [
    { title: 'ИНН', dataIndex: 'inn', width: 130, sorter: (a, b) => a.inn.localeCompare(b.inn) },
    { title: 'Наименование', dataIndex: 'name', sorter: (a, b) => a.name.localeCompare(b.name) },
    { title: 'Адрес', dataIndex: 'address', sorter: (a, b) => (a.address ?? '').localeCompare(b.address ?? '') },
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

  const selectedItems = executors.filter(e => selectedRowKeys.includes(e.id));
  const bulkBar = selectedRowKeys.length > 0 && (
    <Space>
      <Typography.Text type="secondary">{selectedRowKeys.length} выбрано</Typography.Text>
      {selectedItems.some(e => !e.isActive) && <Button size="small" loading={bulkLoading} onClick={() => handleBulkStatus(true)}>Активировать</Button>}
      {selectedItems.some(e => e.isActive) && <Button size="small" danger loading={bulkLoading} onClick={() => handleBulkStatus(false)}>Деактивировать</Button>}
    </Space>
  );

  return (
    <Card
      title={<Typography.Title level={4} style={{ margin: 0 }}>Исполнители</Typography.Title>}
      extra={
        <Space>
          {bulkBar}
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>Добавить</Button>
        </Space>
      }
    >
      <Table
        dataSource={executors}
        columns={columns}
        rowKey="id"
        loading={loading}
        size="small"
        pagination={PAGINATION}
        showSorterTooltip={false}
        rowSelection={{ selectedRowKeys, onChange: setSelectedRowKeys }}
      />
      <Modal
        title={editing ? 'Редактировать исполнителя' : 'Новый исполнитель'}
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
          <Form.Item name="name" label="Наименование" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="address" label="Адрес"><Input /></Form.Item>
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
