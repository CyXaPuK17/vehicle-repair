import { useState } from 'react';
import { Table, Card, Typography, Statistic } from 'antd';
import type { ColumnsType, ExpandableConfig } from 'antd/es/table/interface';
import dayjs from 'dayjs';
import ReportFilter from '../../../components/common/ReportFilter';
import { reportByVehicle, exportReport } from '../../../api/reports';
import { downloadBlob } from '../../../components/common/downloadBlob';
import type { ReportByVehicleRow, ReportByVehicleRepairRow } from '../../../types';
import { PAGINATION } from '../../../utils/pagination';

const repairColumns: ColumnsType<ReportByVehicleRepairRow> = [
  { title: 'Вид ремонта', dataIndex: 'repairTypeName', sorter: (a, b) => a.repairTypeName.localeCompare(b.repairTypeName) },
  { title: 'Исполнитель', dataIndex: 'executorName', sorter: (a, b) => a.executorName.localeCompare(b.executorName) },
  {
    title: 'Дата', dataIndex: 'receivedAt',
    sorter: (a, b) => new Date(a.receivedAt).getTime() - new Date(b.receivedAt).getTime(),
    render: (v: string) => dayjs(v).format('DD.MM.YYYY'),
  },
  {
    title: 'Пробег', dataIndex: 'mileage',
    sorter: (a, b) => a.mileage - b.mileage,
    render: (v: number) => `${v.toLocaleString('ru')} км`,
  },
  {
    title: 'Стоимость', dataIndex: 'cost', align: 'right' as const,
    sorter: (a, b) => a.cost - b.cost,
    render: (v: number) => `${v.toLocaleString('ru', { minimumFractionDigits: 2 })} ₽`,
  },
];

const columns: ColumnsType<ReportByVehicleRow> = [
  { title: 'Гос. номер', dataIndex: 'licensePlate', width: 120, sorter: (a, b) => a.licensePlate.localeCompare(b.licensePlate) },
  { title: 'ТС', dataIndex: 'makeModel', sorter: (a, b) => a.makeModel.localeCompare(b.makeModel) },
  { title: 'Заказчик', dataIndex: 'customerName', sorter: (a, b) => a.customerName.localeCompare(b.customerName) },
  {
    title: 'Сумма', dataIndex: 'totalCost', align: 'right',
    sorter: (a, b) => a.totalCost - b.totalCost,
    render: (v: number) => `${v.toLocaleString('ru', { minimumFractionDigits: 2 })} ₽`,
  },
  {
    title: 'Пробег за период', dataIndex: 'mileageDelta',
    sorter: (a, b) => a.mileageDelta - b.mileageDelta,
    render: (v: number) => `${v.toLocaleString('ru')} км`,
  },
];

const expandable: ExpandableConfig<ReportByVehicleRow> = {
  expandedRowRender: (record) => (
    <Table
      dataSource={record.repairs}
      columns={repairColumns}
      rowKey={(r) => r.receivedAt + r.repairTypeName}
      size="small"
      pagination={false}
      showSorterTooltip={false}
      style={{ marginLeft: 48 }}
    />
  ),
};

export default function ReportByVehiclePage() {
  const [rows, setRows] = useState<ReportByVehicleRow[]>([]);
  const [loading, setLoading] = useState(false);
  const [range, setRange] = useState<[string, string] | null>(null);

  const handleSearch = async (from: string, to: string) => {
    setRange([from, to]);
    setLoading(true);
    try {
      const res = await reportByVehicle(from, to);
      setRows(res.data.data!.rows);
    } finally {
      setLoading(false);
    }
  };

  const handleExport = async (format: 'xlsx' | 'pdf') => {
    if (!range) return;
    const res = await exportReport('by-vehicle', format, range[0], range[1]);
    downloadBlob(res.data as Blob, `Отчёт_по_ТС.${format}`);
  };

  const totalCost = rows.reduce((s, r) => s + r.totalCost, 0);

  return (
    <Card title={<Typography.Title level={4} style={{ margin: 0 }}>Отчёт по транспортному средству за период</Typography.Title>}>
      <ReportFilter onSearch={handleSearch} onExport={range ? handleExport : undefined} loading={loading} />
      {rows.length > 0 && (
        <Statistic title="Общая сумма по всем ТС" value={totalCost} precision={2} suffix="₽" style={{ marginBottom: 16 }} />
      )}
      <Table
        dataSource={rows}
        columns={columns}
        rowKey="vehicleId"
        loading={loading}
        size="small"
        expandable={expandable}
        pagination={PAGINATION}
        showSorterTooltip={false}
      />
    </Card>
  );
}
