import { useState } from 'react';
import { Table, Card, Typography, Statistic, Row, Col, Tag } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import dayjs from 'dayjs';
import ReportFilter from '../../../components/common/ReportFilter';
import { reportByRepairs, exportReport } from '../../../api/reports';
import { downloadBlob } from '../../../components/common/downloadBlob';
import type { ReportByRepairsRow } from '../../../types';
import { PAGINATION } from '../../../utils/pagination';

const columns: ColumnsType<ReportByRepairsRow> = [
  { title: 'Гос. номер', dataIndex: 'licensePlate', key: 'licensePlate', width: 110, sorter: (a, b) => a.licensePlate.localeCompare(b.licensePlate) },
  { title: 'ТС', dataIndex: 'vehicleMakeModel', key: 'vehicle', sorter: (a, b) => a.vehicleMakeModel.localeCompare(b.vehicleMakeModel) },
  { title: 'Заказчик', dataIndex: 'customerName', key: 'customer', sorter: (a, b) => a.customerName.localeCompare(b.customerName) },
  { title: 'Исполнитель', dataIndex: 'executorName', key: 'executor', sorter: (a, b) => a.executorName.localeCompare(b.executorName) },
  { title: 'Вид ремонта', dataIndex: 'repairTypeName', key: 'repairType', sorter: (a, b) => a.repairTypeName.localeCompare(b.repairTypeName) },
  {
    title: 'Дата приёмки', dataIndex: 'receivedAt', key: 'received',
    sorter: (a, b) => new Date(a.receivedAt).getTime() - new Date(b.receivedAt).getTime(),
    render: (v: string) => dayjs(v).format('DD.MM.YYYY'),
  },
  {
    title: 'Дата выдачи', dataIndex: 'issuedAt', key: 'issued',
    sorter: (a, b) => new Date(a.issuedAt ?? 0).getTime() - new Date(b.issuedAt ?? 0).getTime(),
    render: (v?: string) => v ? dayjs(v).format('DD.MM.YYYY') : <Tag>Не выдано</Tag>,
  },
  {
    title: 'Пробег', dataIndex: 'mileage', key: 'mileage',
    sorter: (a, b) => a.mileage - b.mileage,
    render: (v: number) => `${v.toLocaleString('ru')} км`,
  },
  {
    title: 'Стоимость', dataIndex: 'cost', key: 'cost',
    sorter: (a, b) => a.cost - b.cost,
    render: (v: number) => `${v.toLocaleString('ru', { minimumFractionDigits: 2 })} ₽`,
    align: 'right',
  },
];

export default function ReportByRepairsPage() {
  const [rows, setRows] = useState<ReportByRepairsRow[]>([]);
  const [totalCost, setTotalCost] = useState(0);
  const [loading, setLoading] = useState(false);
  const [range, setRange] = useState<[string, string] | null>(null);

  const handleSearch = async (from: string, to: string) => {
    setRange([from, to]);
    setLoading(true);
    try {
      const res = await reportByRepairs(from, to);
      setRows(res.data.data!.rows);
      setTotalCost(res.data.data!.totalCost);
    } finally {
      setLoading(false);
    }
  };

  const handleExport = async (format: 'xlsx' | 'pdf') => {
    if (!range) return;
    const res = await exportReport('by-repairs', format, range[0], range[1]);
    downloadBlob(res.data as Blob, `Отчёт_по_ремонтам.${format}`);
  };

  return (
    <Card title={<Typography.Title level={4} style={{ margin: 0 }}>Отчёт по ремонтам за период</Typography.Title>}>
      <ReportFilter onSearch={handleSearch} onExport={range ? handleExport : undefined} loading={loading} />
      {rows.length > 0 && (
        <Row gutter={16} style={{ marginBottom: 16 }}>
          <Col>
            <Statistic title="Кол-во ремонтов" value={rows.length} />
          </Col>
          <Col>
            <Statistic title="Сумма" value={totalCost} precision={2} suffix="₽" />
          </Col>
        </Row>
      )}
      <Table
        dataSource={rows}
        columns={columns}
        rowKey="repairId"
        loading={loading}
        size="small"
        pagination={PAGINATION}
        showSorterTooltip={false}
        scroll={{ x: 1000 }}
      />
    </Card>
  );
}
