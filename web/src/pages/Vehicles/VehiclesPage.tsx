import { useEffect, useState } from 'react';
import { Table, Card, Button, Modal, Form, Input, Select, InputNumber, Switch, Typography, message, Space } from 'antd';
import { PlusOutlined, EditOutlined, HistoryOutlined, SearchOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { getVehicles, createVehicle, updateVehicle } from '../../api/vehicles';
import { getCustomers } from '../../api/customers';
import type { VehicleDto, CustomerDto } from '../../types';
import StatusTag from '../../components/common/StatusTag';
import { PAGINATION } from '../../utils/pagination';
import VehicleHistoryDrawer from '../../components/VehicleHistoryDrawer/VehicleHistoryDrawer';

const VEHICLE_TYPES = [
  { value: 'Passenger', label: 'Легковой' },
  { value: 'Truck', label: 'Грузовой' },
  { value: 'Bus', label: 'Автобус' },
  { value: 'Special', label: 'Специальный' },
];
const VEHICLE_TYPE_LABELS = Object.fromEntries(VEHICLE_TYPES.map(t => [t.value, t.label])) as Record<string, string>;

export default function VehiclesPage() {
  const [vehicles, setVehicles] = useState<VehicleDto[]>([]);
  const [customers, setCustomers] = useState<CustomerDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<VehicleDto | null>(null);
  const [form] = Form.useForm();
  const [search, setSearch] = useState('');
  const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);
  const [bulkLoading, setBulkLoading] = useState(false);
  const [historyVehicle, setHistoryVehicle] = useState<VehicleDto | null>(null);

  const load = async () => {
    try {
      const [vRes, cRes] = await Promise.all([getVehicles(), getCustomers()]);
      setVehicles(vRes.data.data!);
      setCustomers(cRes.data.data!.filter(c => c.isActive));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const openCreate = () => {
    setEditing(null);
    form.resetFields();
    setModalOpen(true);
  };

  const openEdit = (v: VehicleDto) => {
    setEditing(v);
    form.setFieldsValue({
      licensePlate: v.licensePlate,
      make: v.make,
      model: v.model,
      year: v.year,
      vin: v.vin,
      vehicleType: v.vehicleType,
      isActive: v.isActive,
    });
    setModalOpen(true);
  };

  const handleOk = async () => {
    const values = await form.validateFields();
    try {
      if (editing) {
        await updateVehicle(editing.id, values);
        message.success('ТС обновлено');
      } else {
        await createVehicle(values);
        message.success('ТС добавлено');
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
      const selected = vehicles.filter(v => selectedRowKeys.includes(v.id));
      await Promise.all(selected.map(v => updateVehicle(v.id, {
        licensePlate: v.licensePlate, make: v.make, model: v.model,
        year: v.year, vin: v.vin, vehicleType: v.vehicleType, isActive,
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

  const columns: ColumnsType<VehicleDto> = [
    { title: 'Гос. номер', dataIndex: 'licensePlate', width: 120, sorter: (a, b) => a.licensePlate.localeCompare(b.licensePlate) },
    { title: 'Марка', dataIndex: 'make', sorter: (a, b) => a.make.localeCompare(b.make) },
    { title: 'Модель', dataIndex: 'model', sorter: (a, b) => a.model.localeCompare(b.model) },
    { title: 'Год', dataIndex: 'year', width: 70, sorter: (a, b) => (a.year ?? 0) - (b.year ?? 0) },
    { title: 'VIN', dataIndex: 'vin', sorter: (a, b) => (a.vin ?? '').localeCompare(b.vin ?? '') },
    {
      title: 'Тип', dataIndex: 'vehicleType',
      sorter: (a, b) => (VEHICLE_TYPE_LABELS[a.vehicleType] ?? a.vehicleType).localeCompare(VEHICLE_TYPE_LABELS[b.vehicleType] ?? b.vehicleType),
      render: (v: string) => VEHICLE_TYPE_LABELS[v] ?? v,
    },
    { title: 'Заказчик', dataIndex: 'customerName', sorter: (a, b) => (a.customerName ?? '').localeCompare(b.customerName ?? '') },
    {
      title: 'Статус', dataIndex: 'isActive',
      sorter: (a, b) => Number(a.isActive) - Number(b.isActive),
      render: (v: boolean) => <StatusTag isActive={v} />,
    },
    {
      title: '', key: 'actions', width: 90,
      render: (_, r) => (
        <Space size={4}>
          <Button size="small" icon={<EditOutlined />} onClick={() => openEdit(r)} />
          <Button size="small" icon={<HistoryOutlined />} onClick={() => setHistoryVehicle(r)} title="История ремонтов" />
        </Space>
      ),
    },
  ];

  const q = search.toLowerCase();
  const filtered = q
    ? vehicles.filter(v => [v.licensePlate, v.make, v.model, v.vin, v.customerName].some(f => f?.toLowerCase().includes(q)))
    : vehicles;

  const selectedItems = vehicles.filter(v => selectedRowKeys.includes(v.id));
  const bulkBar = selectedRowKeys.length > 0 && (
    <Space>
      <Typography.Text type="secondary">{selectedRowKeys.length} выбрано</Typography.Text>
      {selectedItems.some(v => !v.isActive) && <Button size="small" loading={bulkLoading} onClick={() => handleBulkStatus(true)}>Активировать</Button>}
      {selectedItems.some(v => v.isActive) && <Button size="small" danger loading={bulkLoading} onClick={() => handleBulkStatus(false)}>Деактивировать</Button>}
    </Space>
  );

  return (
    <Card
      title={<Typography.Title level={4} style={{ margin: 0 }}>Транспортные средства</Typography.Title>}
      extra={
        <Space>
          {bulkBar}
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>Добавить</Button>
        </Space>
      }
    >
      <Input
        prefix={<SearchOutlined />}
        placeholder="Поиск по гос. номеру, марке, модели, VIN, заказчику..."
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
        title={editing ? 'Редактировать ТС' : 'Новое ТС'}
        open={modalOpen}
        onOk={handleOk}
        onCancel={() => setModalOpen(false)}
        okText="Сохранить"
      >
        <Form form={form} layout="vertical">
          <Form.Item name="licensePlate" label="Гос. номер" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="make" label="Марка" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="model" label="Модель" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="year" label="Год выпуска"><InputNumber style={{ width: '100%' }} min={1900} max={2099} /></Form.Item>
          <Form.Item name="vin" label="VIN"><Input maxLength={17} /></Form.Item>
          <Form.Item name="vehicleType" label="Тип ТС" rules={[{ required: true }]}>
            <Select options={VEHICLE_TYPES} />
          </Form.Item>
          {!editing && (
            <Form.Item name="customerId" label="Заказчик" rules={[{ required: true }]}>
              <Select
                showSearch
                options={customers.map(c => ({ value: c.id, label: `${c.name} (${c.inn})` }))}
                filterOption={(input, opt) => (opt?.label as string ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item>
          )}
          {editing && (
            <Form.Item name="isActive" label="Активен" valuePropName="checked">
              <Switch />
            </Form.Item>
          )}
        </Form>
      </Modal>
      <VehicleHistoryDrawer
        vehicleId={historyVehicle?.id ?? null}
        title={historyVehicle ? `${historyVehicle.licensePlate} · ${historyVehicle.make} ${historyVehicle.model}` : ''}
        onClose={() => setHistoryVehicle(null)}
      />
    </Card>
  );
}
