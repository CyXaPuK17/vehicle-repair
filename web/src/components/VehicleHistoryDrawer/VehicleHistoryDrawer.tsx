import { useEffect, useState } from 'react';
import { Drawer, Table, Tag, Typography, Alert, Spin } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import dayjs from 'dayjs';
import { getVehicleHistory } from '../../api/vehicles';
import type { RepairDto, RepairStatus } from '../../types';
import { RepairStatusTag } from '../../utils/repairStatus';

const columns: ColumnsType<RepairDto> = [
  {
    title: 'Дата приёмки', dataIndex: 'receivedAt', width: 120,
    defaultSortOrder: 'descend',
    sorter: (a, b) => new Date(a.receivedAt).getTime() - new Date(b.receivedAt).getTime(),
    render: (v: string) => dayjs(v).format('DD.MM.YYYY'),
  },
  { title: 'Вид ремонта', dataIndex: 'repairTypeName', sorter: (a, b) => a.repairTypeName.localeCompare(b.repairTypeName) },
  { title: 'Исполнитель', dataIndex: 'executorName', sorter: (a, b) => a.executorName.localeCompare(b.executorName) },
  {
    title: 'Статус', dataIndex: 'status', width: 110,
    render: (v: RepairStatus) => <RepairStatusTag status={v} />,
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

interface Props {
  vehicleId: string | null;
  title: string;
  onClose: () => void;
}

export default function VehicleHistoryDrawer({ vehicleId, title, onClose }: Props) {
  const [rows, setRows]     = useState<RepairDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError]   = useState<string | null>(null);

  useEffect(() => {
    if (!vehicleId) return;
    setRows([]);
    setError(null);
    setLoading(true);
    getVehicleHistory(vehicleId)
      .then(res => setRows(res.data.data ?? []))
      .catch(e => setError(e?.response?.data?.message ?? 'Ошибка загрузки'))
      .finally(() => setLoading(false));
  }, [vehicleId]);

  const totalCost = rows.reduce((s, r) => s + r.cost, 0);

  return (
    <Drawer
      open={!!vehicleId}
      onClose={onClose}
      title={<>История ремонтов<Typography.Text type="secondary" style={{ marginLeft: 8, fontWeight: 400 }}>{title}</Typography.Text></>}
      width={800}
      destroyOnHide
    >
      {loading && <Spin style={{ display: 'block', margin: '40px auto' }} />}
      {error   && <Alert type="error" message={error} showIcon />}
      {!loading && !error && (
        <>
          {rows.length > 0 && (
            <div style={{ marginBottom: 12, display: 'flex', gap: 24 }}>
              <Typography.Text type="secondary">Всего ремонтов: <b>{rows.length}</b></Typography.Text>
              <Typography.Text type="secondary">
                Сумма: <b>{totalCost.toLocaleString('ru', { minimumFractionDigits: 2 })} ₽</b>
              </Typography.Text>
            </div>
          )}
          <Table
            dataSource={rows}
            columns={columns}
            rowKey="id"
            size="small"
            pagination={{ pageSize: 10, showSizeChanger: false, hideOnSinglePage: true }}
            showSorterTooltip={false}
            locale={{ emptyText: 'Ремонтов нет' }}
          />
        </>
      )}
    </Drawer>
  );
}
