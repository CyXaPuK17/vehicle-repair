import { useEffect, useState } from 'react';
import { Table, Card, Typography, Tag, Row, Col, Statistic, Input, theme } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import dayjs from 'dayjs';
import { SyncOutlined, CheckCircleOutlined, CalendarOutlined, DollarOutlined, SearchOutlined } from '@ant-design/icons';
import { getRepairs } from '../../api/repairs';
import { getMyStats, type ExecutorStatsDto } from '../../api/executors';
import type { RepairDto, RepairStatus } from '../../types';
import { RepairStatusTag } from '../../utils/repairStatus';
import { PAGINATION } from '../../utils/pagination';

const columns: ColumnsType<RepairDto> = [
  { title: 'Гос. номер', dataIndex: 'licensePlate', width: 110, sorter: (a, b) => a.licensePlate.localeCompare(b.licensePlate) },
  { title: 'ТС', dataIndex: 'vehicleMakeModel', sorter: (a, b) => a.vehicleMakeModel.localeCompare(b.vehicleMakeModel) },
  { title: 'Заказчик', dataIndex: 'customerName', sorter: (a, b) => a.customerName.localeCompare(b.customerName) },
  { title: 'Вид ремонта', dataIndex: 'repairTypeName', sorter: (a, b) => a.repairTypeName.localeCompare(b.repairTypeName) },
  {
    title: 'Статус', dataIndex: 'status', width: 110,
    sorter: (a, b) => a.status.localeCompare(b.status),
    render: (v: RepairStatus) => <RepairStatusTag status={v} />,
  },
  {
    title: 'Дата приёмки', dataIndex: 'receivedAt', width: 120,
    defaultSortOrder: 'descend',
    sorter: (a, b) => new Date(a.receivedAt).getTime() - new Date(b.receivedAt).getTime(),
    render: (v: string) => dayjs(v).format('DD.MM.YYYY'),
  },
  {
    title: 'Стоимость', dataIndex: 'cost', width: 130, align: 'right',
    sorter: (a, b) => a.cost - b.cost,
    render: (v: number) => `${v.toLocaleString('ru', { minimumFractionDigits: 2 })} ₽`,
  },
];

const now = dayjs();
const monthName = now.format('MMMM');
const yearNum   = now.year();

export default function QueuePage() {
  const { token } = theme.useToken();
  const [rows,    setRows]    = useState<RepairDto[]>([]);
  const [search,  setSearch]  = useState('');
  const [stats,   setStats]   = useState<ExecutorStatsDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([
      getRepairs('2000-01-01', '2100-01-01', 1, 200)
        .then(res => setRows(res.data.data!.items.filter(r => r.status !== 'Issued'))),
      getMyStats()
        .then(res => setStats(res.data.data!))
        .catch(() => {}),
    ]).finally(() => setLoading(false));
  }, []);

  const q = search.toLowerCase();
  const filtered = q
    ? rows.filter(r => [r.licensePlate, r.vehicleMakeModel, r.customerName, r.repairTypeName].some(v => v?.toLowerCase().includes(q)))
    : rows;

  return (
    <>
      {stats && (
        <>
          <Row gutter={[16, 16]} style={{ marginBottom: 16 }}>
            <Col xs={24} sm={8}>
              <Card>
                <Statistic
                  title="Активных сейчас"
                  value={stats.activeNow}
                  prefix={<SyncOutlined spin={stats.activeNow > 0} />}
                  valueStyle={{ color: '#fa8c16' }}
                />
              </Card>
            </Col>
            <Col xs={24} sm={8}>
              <Card>
                <Statistic
                  title={`Выполнено за ${monthName}`}
                  value={stats.doneThisMonth}
                  prefix={<CheckCircleOutlined />}
                  valueStyle={{ color: '#13c2c2' }}
                />
              </Card>
            </Col>
            <Col xs={24} sm={8}>
              <Card>
                <Statistic
                  title={`Выполнено за ${yearNum} год`}
                  value={stats.doneThisYear}
                  prefix={<CalendarOutlined />}
                />
              </Card>
            </Col>
          </Row>
          <Row gutter={[16, 16]} style={{ marginBottom: 16 }}>
            <Col xs={24} sm={12}>
              <Card>
                <Statistic
                  title={`Выручка за ${monthName}`}
                  value={stats.revenueThisMonth}
                  precision={2}
                  suffix="₽"
                  prefix={<DollarOutlined />}
                  valueStyle={{ color: '#52c41a' }}
                />
              </Card>
            </Col>
            <Col xs={24} sm={12}>
              <Card>
                <Statistic
                  title={`Выручка за ${yearNum} год`}
                  value={stats.revenueThisYear}
                  precision={2}
                  suffix="₽"
                  prefix={<DollarOutlined />}
                  valueStyle={{ color: '#52c41a' }}
                />
              </Card>
            </Col>
          </Row>
        </>
      )}

      <Card title={
        <Typography.Title level={4} style={{ margin: 0 }}>
          Мои задания
          {!loading && rows.length === 0 && (
            <Tag color="green" style={{ marginLeft: 12, fontWeight: 400 }}>Нет активных заданий</Tag>
          )}
        </Typography.Title>
      }>
        <Input
          prefix={<SearchOutlined />}
          placeholder="Поиск по гос. номеру, ТС, заказчику, виду ремонта..."
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
      </Card>
    </>
  );
}
