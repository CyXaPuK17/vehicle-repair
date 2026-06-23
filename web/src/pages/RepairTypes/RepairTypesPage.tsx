import { useEffect, useState } from 'react';
import { Table, Card, Button, Modal, Form, Input, Switch, Typography, message, Space } from 'antd';
import { PlusOutlined, EditOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { getRepairTypes, createRepairType, updateRepairType } from '../../api/repairTypes';
import type { RepairTypeDto } from '../../types';
import StatusTag from '../../components/common/StatusTag';
import { PAGINATION } from '../../utils/pagination';

export default function RepairTypesPage() {
  const [items, setItems] = useState<RepairTypeDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<RepairTypeDto | null>(null);
  const [form] = Form.useForm();
  const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);
  const [bulkLoading, setBulkLoading] = useState(false);

  const load = async () => {
    try {
      const res = await getRepairTypes();
      setItems(res.data.data!);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const openCreate = () => { setEditing(null); form.resetFields(); setModalOpen(true); };
  const openEdit = (item: RepairTypeDto) => {
    setEditing(item);
    form.setFieldsValue({ name: item.name, description: item.description, isActive: item.isActive });
    setModalOpen(true);
  };

  const handleOk = async () => {
    const values = await form.validateFields();
    try {
      if (editing) {
        await updateRepairType(editing.id, values);
        message.success('Вид ремонта обновлён');
      } else {
        await createRepairType(values);
        message.success('Вид ремонта добавлен');
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
      const selected = items.filter(r => selectedRowKeys.includes(r.id));
      await Promise.all(selected.map(r => updateRepairType(r.id, {
        name: r.name, description: r.description, isActive,
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

  const columns: ColumnsType<RepairTypeDto> = [
    { title: 'Наименование', dataIndex: 'name', sorter: (a, b) => a.name.localeCompare(b.name) },
    { title: 'Описание', dataIndex: 'description', sorter: (a, b) => (a.description ?? '').localeCompare(b.description ?? '') },
    {
      title: 'Статус', dataIndex: 'isActive',
      sorter: (a, b) => Number(a.isActive) - Number(b.isActive),
      render: (v: boolean) => <StatusTag isActive={v} />,
    },
    {
      title: '', key: 'actions', width: 48,
      render: (_, r) => <Button size="small" icon={<EditOutlined />} onClick={() => openEdit(r)} />,
    },
  ];

  const selectedItems = items.filter(r => selectedRowKeys.includes(r.id));
  const bulkBar = selectedRowKeys.length > 0 && (
    <Space>
      <Typography.Text type="secondary">{selectedRowKeys.length} выбрано</Typography.Text>
      {selectedItems.some(r => !r.isActive) && <Button size="small" loading={bulkLoading} onClick={() => handleBulkStatus(true)}>Активировать</Button>}
      {selectedItems.some(r => r.isActive) && <Button size="small" danger loading={bulkLoading} onClick={() => handleBulkStatus(false)}>Деактивировать</Button>}
    </Space>
  );

  return (
    <Card
      title={<Typography.Title level={4} style={{ margin: 0 }}>Виды ремонта</Typography.Title>}
      extra={
        <Space>
          {bulkBar}
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>Добавить</Button>
        </Space>
      }
    >
      <Table
        dataSource={items}
        columns={columns}
        rowKey="id"
        loading={loading}
        size="small"
        pagination={PAGINATION}
        showSorterTooltip={false}
        rowSelection={{ selectedRowKeys, onChange: setSelectedRowKeys }}
      />
      <Modal
        title={editing ? 'Редактировать вид ремонта' : 'Новый вид ремонта'}
        open={modalOpen}
        onOk={handleOk}
        onCancel={() => setModalOpen(false)}
        okText="Сохранить"
      >
        <Form form={form} layout="vertical">
          <Form.Item name="name" label="Наименование" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="description" label="Описание"><Input.TextArea rows={3} /></Form.Item>
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
