import { useEffect, useState } from 'react';
import { Table, Card, Typography, Select, Form, DatePicker, Space, Tag } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import dayjs from 'dayjs';
import { getRepairs } from '../../api/repairs';
import type { RepairDto, RepairStatus } from '../../types';
import { RepairStatusTag, REPAIR_STATUS_OPTIONS } from '../../utils/repairStatus';
import { PAGINATION } from '../../utils/pagination';

const { RangePicker } = DatePicker;

const columns: ColumnsType<RepairDto> = [
  { title: 'Гос. номер', dataIndex: 'licensePlate', width: 110, sorter: (a, b) => a.licensePlate.localeCompare(b.licensePlate) },
  { title: 'ТС', dataIndex: 'vehicleMakeModel', sorter: (a, b) => a.vehicleMakeModel.localeCompare(b.vehicleMakeModel) },
  { title: 'Заказчик', dataIndex: 'customerName', sorter: (a, b) => a.customerName.localeCompare(b.customerName) },
  { title: 'Исполнитель', dataIndex: 'executorName', sorter: (a, b) => a.executorName.localeCompare(b.executorName) },
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
];

export default function RepairsPage() {
  const [allRows, setAllRows]       = useState<RepairDto[]>([]);
  const [statusFilter, setStatusFilter] = useState<RepairStatus[]>([]);
  const [loading, setLoading]       = useState(false);
  const [range, setRange]           = useState<[dayjs.Dayjs, dayjs.Dayjs]>([
    dayjs().startOf('year'), dayjs(),
  ]);

  useEffect(() => {
    setLoading(true);
    const [from, to] = range;
    getRepairs(from.startOf('day').toISOString(), to.endOf('day').toISOString(), 1, 200)
      .then(res => setAllRows(res.data.data!.items))
      .finally(() => setLoading(false));
  }, [range]);

  const rows = statusFilter.length
    ? allRows.filter(r => statusFilter.includes(r.status))
    : allRows;

  return (
    <Card title={<Typography.Title level={4} style={{ margin: 0 }}>Ремонты</Typography.Title>}>
      <Form layout="inline" style={{ marginBottom: 16 }}>
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
      {!loading && allRows.length > 0 && statusFilter.length > 0 && (
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
      />
    </Card>
  );
}
