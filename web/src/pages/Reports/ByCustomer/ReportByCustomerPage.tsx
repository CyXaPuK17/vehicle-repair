import { useState } from 'react';
import { Table, Card, Typography, Statistic, Row, Col } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import ReportFilter from '../../../components/common/ReportFilter';
import { reportByCustomer, exportReport } from '../../../api/reports';
import { downloadBlob } from '../../../components/common/downloadBlob';
import type { ReportByCustomerRow } from '../../../types';
import { PAGINATION } from '../../../utils/pagination';

const columns: ColumnsType<ReportByCustomerRow> = [
  { title: 'Заказчик', dataIndex: 'customerName', key: 'name', sorter: (a, b) => a.customerName.localeCompare(b.customerName) },
  { title: 'ИНН', dataIndex: 'customerINN', key: 'inn', width: 130, sorter: (a, b) => a.customerINN.localeCompare(b.customerINN) },
  { title: 'Кол-во ремонтов', dataIndex: 'repairCount', key: 'count', align: 'center', sorter: (a, b) => a.repairCount - b.repairCount },
  {
    title: 'Сумма', dataIndex: 'totalCost', key: 'cost', align: 'right',
    sorter: (a, b) => a.totalCost - b.totalCost,
    render: (v: number) => `${v.toLocaleString('ru', { minimumFractionDigits: 2 })} ₽`,
  },
];

export default function ReportByCustomerPage() {
  const [rows, setRows] = useState<ReportByCustomerRow[]>([]);
  const [totals, setTotals] = useState({ count: 0, cost: 0 });
  const [loading, setLoading] = useState(false);
  const [range, setRange] = useState<[string, string] | null>(null);

  const handleSearch = async (from: string, to: string) => {
    setRange([from, to]);
    setLoading(true);
    try {
      const res = await reportByCustomer(from, to);
      setRows(res.data.data!.rows);
      setTotals({ count: res.data.data!.totalCount, cost: res.data.data!.totalCost });
    } finally {
      setLoading(false);
    }
  };

  const handleExport = async (format: 'xlsx' | 'pdf') => {
    if (!range) return;
    const res = await exportReport('by-customer', format, range[0], range[1]);
    downloadBlob(res.data as Blob, `Отчёт_по_заказчику.${format}`);
  };

  return (
    <Card title={<Typography.Title level={4} style={{ margin: 0 }}>Отчёт по заказчику за период</Typography.Title>}>
      <ReportFilter onSearch={handleSearch} onExport={range ? handleExport : undefined} loading={loading} />
      {rows.length > 0 && (
        <Row gutter={16} style={{ marginBottom: 16 }}>
          <Col><Statistic title="Всего ремонтов" value={totals.count} /></Col>
          <Col><Statistic title="Общая сумма" value={totals.cost} precision={2} suffix="₽" /></Col>
        </Row>
      )}
      <Table
        dataSource={rows}
        columns={columns}
        rowKey="customerId"
        loading={loading}
        size="small"
        pagination={PAGINATION}
        showSorterTooltip={false}
      />
    </Card>
  );
}
