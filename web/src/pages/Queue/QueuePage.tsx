import { useEffect, useState } from 'react';
import { Table, Card, Typography, Tag, Row, Col, Statistic, Input, theme, Button, Popconfirm, DatePicker, Space, message } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import dayjs, { type Dayjs } from 'dayjs';
import { SyncOutlined, CheckCircleOutlined, CalendarOutlined, DollarOutlined, SearchOutlined } from '@ant-design/icons';
import { getRepairs, startRepair, completeRepair, issueRepair } from '../../api/repairs';
import { getMyStats, type ExecutorStatsDto } from '../../api/executors';
import type { RepairDto, RepairStatus } from '../../types';
import { RepairStatusTag } from '../../utils/repairStatus';
import { PAGINATION } from '../../utils/pagination';
import InactiveHint from '../../components/common/InactiveHint';

function IssueAction({ repair, onIssued }: { repair: RepairDto; onIssued: () => void }) {
  const [issuedAt, setIssuedAt] = useState<Dayjs>(() => dayjs());
  const [loading, setLoading] = useState(false);

  const handleIssue = async () => {
    setLoading(true);
    try {
      await issueRepair(repair.id, issuedAt.toISOString());
      message.success('ТС выдано из ремонта');
      onIssued();
    } catch {
      message.error('Ошибка при выдаче');
      onIssued(); // resync with server state in case of a stale/conflicting status
    } finally {
      setLoading(false);
    }
  };

  return (
    <Space size="small">
      <DatePicker size="small" value={issuedAt} onChange={(v) => v && setIssuedAt(v)} format="DD.MM.YYYY" allowClear={false} />
      <Popconfirm title="Выдать ТС из ремонта?" onConfirm={handleIssue} okText="Да" cancelText="Отмена">
        <Button size="small" type="primary" loading={loading}>Выдать</Button>
      </Popconfirm>
    </Space>
  );
}

export default function QueuePage() {
  const { token } = theme.useToken();
  const [rows,    setRows]    = useState<RepairDto[]>([]);
  const [search,  setSearch]  = useState('');
  const [stats,   setStats]   = useState<ExecutorStatsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [actionLoadingId, setActionLoadingId] = useState<string | null>(null);

  const now = dayjs();
  const monthName = now.format('MMMM');
  const yearNum   = now.year();

  const loadData = () => {
    setLoading(true);
    return Promise.all([
      getRepairs('2000-01-01', '2100-01-01', 1, 200)
        .then(res => setRows(res.data.data!.items.filter(r => r.status !== 'Issued')))
        .catch(() => message.error('Ошибка загрузки заданий')),
      getMyStats()
        .then(res => setStats(res.data.data!))
        .catch(() => {}),
    ]).finally(() => setLoading(false));
  };

  useEffect(() => {
    loadData();
  }, []);

  const runAction = async (id: string, action: (id: string) => Promise<unknown>, successMsg: string) => {
    setActionLoadingId(id);
    try {
      await action(id);
      message.success(successMsg);
      await loadData();
    } catch {
      message.error('Ошибка при выполнении действия');
      await loadData(); // resync with server state in case of a stale/conflicting status
    } finally {
      setActionLoadingId(null);
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
    {
      title: 'Действие', key: 'action', width: 320,
      render: (_, repair) => (
        <Space size="small">
          {repair.status === 'Received' && (
            <Popconfirm title="Взять ремонт в работу?" onConfirm={() => runAction(repair.id, startRepair, 'Ремонт взят в работу')} okText="Да" cancelText="Отмена">
              <Button size="small" type="primary" loading={actionLoadingId === repair.id}>Начать</Button>
            </Popconfirm>
          )}
          {repair.status === 'InProgress' && (
            <Popconfirm title="Завершить ремонт?" onConfirm={() => runAction(repair.id, completeRepair, 'Ремонт завершён')} okText="Да" cancelText="Отмена">
              <Button size="small" type="primary" loading={actionLoadingId === repair.id}>Завершить</Button>
            </Popconfirm>
          )}
          {/* Выдать разрешено с любого невыданного статуса — так же, как и в десктоп-приложении */}
          <IssueAction repair={repair} onIssued={loadData} />
        </Space>
      ),
    },
  ];

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
