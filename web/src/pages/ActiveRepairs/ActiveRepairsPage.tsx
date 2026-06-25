import { useEffect, useState } from 'react';
import { Table, Card, Typography, Tag, Button, Space, Input, theme } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import dayjs from 'dayjs';
import { HistoryOutlined, SearchOutlined } from '@ant-design/icons';
import { getRepairs } from '../../api/repairs';
import type { RepairDto, RepairStatus } from '../../types';
import { RepairStatusTag } from '../../utils/repairStatus';
import { PAGINATION } from '../../utils/pagination';
import VehicleHistoryDrawer from '../../components/VehicleHistoryDrawer/VehicleHistoryDrawer';

export default function ActiveRepairsPage() {
  const { token } = theme.useToken();
  const [rows, setRows]           = useState<RepairDto[]>([]);
  const [search, setSearch]       = useState('');
  const [loading, setLoading]     = useState(true);
  const [historyItem, setHistoryItem] = useState<RepairDto | null>(null);

  useEffect(() => {
    getRepairs(undefined, undefined, 1, 200)
      .then(res => setRows(res.data.data!.items))
      .finally(() => setLoading(false));
  }, []);

  const columns: ColumnsType<RepairDto> = [
    { title: 'Гос. номер', dataIndex: 'licensePlate', width: 110, sorter: (a, b) => a.licensePlate.localeCompare(b.licensePlate) },
    { title: 'ТС', dataIndex: 'vehicleMakeModel', sorter: (a, b) => a.vehicleMakeModel.localeCompare(b.vehicleMakeModel) },
    { title: 'Вид ремонта', dataIndex: 'repairTypeName', sorter: (a, b) => a.repairTypeName.localeCompare(b.repairTypeName) },
    { title: 'Исполнитель', dataIndex: 'executorName', sorter: (a, b) => a.executorName.localeCompare(b.executorName) },
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
      title: 'Стоимость', dataIndex: 'cost', width: 130, align: 'right',
      sorter: (a, b) => a.cost - b.cost,
      render: (v: number) => `${v.toLocaleString('ru', { minimumFractionDigits: 2 })} ₽`,
    },
    {
      title: '', key: 'actions', width: 48,
      render: (_, r) => (
        <Button
          size="small" icon={<HistoryOutlined />}
          title="История ремонтов этого ТС"
          onClick={() => setHistoryItem(r)}
        />
      ),
    },
  ];

  const q = search.toLowerCase();
  const filtered = q
    ? rows.filter(r => [r.licensePlate, r.vehicleMakeModel, r.repairTypeName, r.executorName].some(v => v?.toLowerCase().includes(q)))
    : rows;

  return (
    <Card title={
      <Typography.Title level={4} style={{ margin: 0 }}>
        Текущие ремонты
        {!loading && rows.length === 0 && (
          <Tag color="green" style={{ marginLeft: 12, fontWeight: 400 }}>Нет активных ремонтов</Tag>
        )}
      </Typography.Title>
    }>
      <Input
        prefix={<SearchOutlined />}
        placeholder="Поиск по гос. номеру, ТС, виду ремонта, исполнителю..."
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
        expandable={{
          expandedRowRender: (r) => (
            <Typography.Text style={{ paddingLeft: 8, color: token.colorTextSecondary }}>
              {r.comment}
            </Typography.Text>
          ),
          rowExpandable: (r) => !!r.comment,
        }}
      />
      <VehicleHistoryDrawer
        vehicleId={historyItem?.vehicleId ?? null}
        title={historyItem ? `${historyItem.licensePlate} · ${historyItem.vehicleMakeModel}` : ''}
        onClose={() => setHistoryItem(null)}
      />
    </Card>
  );
}
