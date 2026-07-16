import { useEffect, useState } from 'react';
import { Table, Card, Typography, Select, Form, DatePicker, Space, Tag, Input, theme, Button, Modal, InputNumber, message } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import dayjs from 'dayjs';
import { SearchOutlined, EditOutlined } from '@ant-design/icons';
import { getRepairs, updateRepair } from '../../api/repairs';
import { getRepairTypes } from '../../api/repairTypes';
import type { RepairDto, RepairStatus, RepairTypeDto } from '../../types';
import { RepairStatusTag, REPAIR_STATUS_OPTIONS } from '../../utils/repairStatus';
import { PAGINATION } from '../../utils/pagination';
import InactiveHint from '../../components/common/InactiveHint';

const { RangePicker } = DatePicker;

export default function RepairsPage() {
  const { token } = theme.useToken();
  const [allRows, setAllRows]       = useState<RepairDto[]>([]);
  const [repairTypes, setRepairTypes] = useState<RepairTypeDto[]>([]);
  const [statusFilter, setStatusFilter] = useState<RepairStatus[]>([]);
  const [search, setSearch]         = useState('');
  const [loading, setLoading]       = useState(false);
  const [range, setRange]           = useState<[dayjs.Dayjs, dayjs.Dayjs]>([
    dayjs().startOf('year'), dayjs(),
  ]);
  const [editing, setEditing] = useState<RepairDto | null>(null);
  const [editForm] = Form.useForm();
  const [saving, setSaving] = useState(false);

  const load = () => {
    setLoading(true);
    const [from, to] = range;
    return getRepairs(from.startOf('day').toISOString(), to.endOf('day').toISOString(), 1, 200)
      .then(res => setAllRows(res.data.data!.items))
      .finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, [range]);

  useEffect(() => {
    getRepairTypes().then(res => setRepairTypes((res.data.data ?? []).filter(t => t.isActive)));
  }, []);

  const openEdit = (r: RepairDto) => {
    setEditing(r);
    editForm.setFieldsValue({
      repairTypeId: r.repairTypeId,
      receivedAt: dayjs(r.receivedAt),
      cost: r.cost,
      mileage: r.mileage,
      comment: r.comment,
    });
  };

  const handleSaveEdit = async () => {
    const values = await editForm.validateFields();
    setSaving(true);
    try {
      await updateRepair(editing!.id, {
        repairTypeId: values.repairTypeId,
        receivedAt: values.receivedAt.toISOString(),
        cost: values.cost,
        mileage: values.mileage,
        comment: values.comment || undefined,
      });
      message.success('Ремонт обновлён');
      setEditing(null);
      await load();
    } catch {
      message.error('Ошибка при сохранении');
    } finally {
      setSaving(false);
    }
  };

  const columns: ColumnsType<RepairDto> = [
    {
      title: 'Гос. номер', dataIndex: 'licensePlate', width: 110, sorter: (a, b) => a.licensePlate.localeCompare(b.licensePlate),
      render: (v: string, r) => <>{v}<InactiveHint active={r.isVehicleActive} /></>,
    },
    { title: 'ТС', dataIndex: 'vehicleMakeModel', sorter: (a, b) => a.vehicleMakeModel.localeCompare(b.vehicleMakeModel) },
    {
      title: 'Заказчик', dataIndex: 'customerName', sorter: (a, b) => a.customerName.localeCompare(b.customerName),
      render: (v: string, r) => <>{v}<InactiveHint active={r.isCustomerActive} /></>,
    },
    {
      title: 'Исполнитель', dataIndex: 'executorName', sorter: (a, b) => a.executorName.localeCompare(b.executorName),
      render: (v: string, r) => <>{v}<InactiveHint active={r.isExecutorActive} /></>,
    },
    { title: 'Вид ремонта', dataIndex: 'repairTypeName', sorter: (a, b) => a.repairTypeName.localeCompare(b.repairTypeName) },
    {
      title: 'Статус', dataIndex: 'status', width: 110,
      sorter: (a, b) => a.status.localeCompare(b.status),
      render: (v: RepairStatus) => <RepairStatusTag status={v} />,
    },
    {
      title: 'Дата приёмки', dataIndex: 'receivedAt', width: 120,
      sorter: (a, b) => new Date(a.receivedAt).getTime() - new Date(b.receivedAt).getTime(),
      render: (v: string) => dayjs(v).format('DD.MM.YYYY'),
    },
    {
      title: 'Дата выдачи', dataIndex: 'issuedAt', width: 120,
      sorter: (a, b) => new Date(a.issuedAt ?? 0).getTime() - new Date(b.issuedAt ?? 0).getTime(),
      render: (v?: string) => v ? dayjs(v).format('DD.MM.YYYY') : <Tag>Не выдано</Tag>,
    },
    {
      title: 'Стоимость', dataIndex: 'cost', width: 130, align: 'right',
      sorter: (a, b) => a.cost - b.cost,
      render: (v: number) => `${v.toLocaleString('ru', { minimumFractionDigits: 2 })} ₽`,
    },
    {
      title: '', key: 'actions', width: 48,
      render: (_, r) => <Button size="small" icon={<EditOutlined />} onClick={() => openEdit(r)} title="Редактировать" />,
    },
  ];

  const byStatus = statusFilter.length
    ? allRows.filter(r => statusFilter.includes(r.status))
    : allRows;
  const q = search.toLowerCase();
  const rows = q
    ? byStatus.filter(r => [r.licensePlate, r.vehicleMakeModel, r.customerName, r.executorName, r.repairTypeName].some(v => v?.toLowerCase().includes(q)))
    : byStatus;

  return (
    <Card title={<Typography.Title level={4} style={{ margin: 0 }}>Ремонты</Typography.Title>}>
      <Form layout="inline" style={{ marginBottom: 12 }}>
        <Form.Item>
          <RangePicker
            format="DD.MM.YYYY"
            value={range}
            onChange={dates => { if (dates?.[0] && dates?.[1]) setRange([dates[0], dates[1]]); }}
          />
        </Form.Item>
        <Form.Item>
          <Select
            mode="multiple"
            placeholder="Все статусы"
            options={REPAIR_STATUS_OPTIONS}
            onChange={setStatusFilter}
            style={{ minWidth: 200 }}
            allowClear
          />
        </Form.Item>
      </Form>
      <Input
        prefix={<SearchOutlined />}
        placeholder="Поиск по гос. номеру, ТС, заказчику, исполнителю, виду ремонта..."
        value={search}
        onChange={e => setSearch(e.target.value)}
        allowClear
        style={{ marginBottom: 12 }}
      />
      {!loading && allRows.length > 0 && (statusFilter.length > 0 || search) && (
        <Space style={{ marginBottom: 12 }}>
          <Typography.Text type="secondary">
            Показано: {rows.length} из {allRows.length}
          </Typography.Text>
        </Space>
      )}
      <Table
        dataSource={rows}
        columns={columns}
        rowKey="id"
        loading={loading}
        size="small"
        pagination={PAGINATION}
        showSorterTooltip={false}
        scroll={{ x: 1000 }}
        expandable={{
          expandedRowRender: (r) => (
            <Typography.Text style={{ paddingLeft: 8, color: token.colorTextSecondary }}>
              {r.comment}
            </Typography.Text>
          ),
          rowExpandable: (r) => !!r.comment,
        }}
      />
      <Modal
        title="Редактировать ремонт"
        open={!!editing}
        onOk={handleSaveEdit}
        onCancel={() => setEditing(null)}
        okText="Сохранить"
        confirmLoading={saving}
      >
        <Form form={editForm} layout="vertical">
          <Form.Item name="repairTypeId" label="Вид ремонта" rules={[{ required: true }]}>
            <Select options={repairTypes.map(t => ({ value: t.id, label: t.name }))} />
          </Form.Item>
          <Form.Item name="receivedAt" label="Дата приёмки" rules={[{ required: true }]}>
            <DatePicker format="DD.MM.YYYY" style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="cost" label="Стоимость (руб.)" rules={[{ required: true }]}>
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="mileage" label="Пробег (км)" rules={[{ required: true }]}>
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="comment" label="Комментарий">
            <Input.TextArea rows={3} />
          </Form.Item>
        </Form>
      </Modal>
    </Card>
  );
}
