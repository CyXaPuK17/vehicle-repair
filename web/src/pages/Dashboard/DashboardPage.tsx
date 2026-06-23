import { useEffect, useState, useCallback } from 'react';
import { Card, Col, Row, Statistic, Table, Typography, Spin, Alert, Space, Select, DatePicker, Segmented } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import type { Dayjs } from 'dayjs';
import dayjs from 'dayjs';
import { ClockCircleOutlined, SyncOutlined, CheckCircleOutlined, CalendarOutlined, DollarOutlined } from '@ant-design/icons';
import { getDashboard, type ExecutorStatDto, type DashboardDto } from '../../api/dashboard';

const { RangePicker } = DatePicker;

type Preset = 'week' | 'month' | 'year' | 'custom';

const LS_PRESET = 'dashboard_period';
const LS_FROM   = 'dashboard_from';
const LS_TO     = 'dashboard_to';
const LS_TOP    = 'dashboard_top_count';

function resolveDates(preset: Preset, customFrom?: Dayjs, customTo?: Dayjs): [Dayjs, Dayjs] {
  const now = dayjs();
  switch (preset) {
    case 'week':  return [now.startOf('isoWeek'), now];
    case 'month': return [now.startOf('month'),   now];
    case 'year':  return [now.startOf('year'),    now];
    case 'custom':
      return [customFrom ?? now.startOf('year'), customTo ?? now];
  }
}

const executorColumns: ColumnsType<ExecutorStatDto> = [
  { title: 'Исполнитель', dataIndex: 'name', sorter: (a, b) => a.name.localeCompare(b.name) },
  { title: 'Ремонтов',    dataIndex: 'count',     width: 110, align: 'right', sorter: (a, b) => a.count - b.count, defaultSortOrder: 'descend' },
  {
    title: 'Сумма', dataIndex: 'totalCost', width: 170, align: 'right',
    sorter: (a, b) => a.totalCost - b.totalCost,
    render: (v: number) => `${v.toLocaleString('ru', { minimumFractionDigits: 2 })} ₽`,
  },
];

export default function DashboardPage() {
  const [preset,     setPreset]     = useState<Preset>((localStorage.getItem(LS_PRESET) as Preset) ?? 'year');
  const [customFrom, setCustomFrom] = useState<Dayjs | undefined>(
    localStorage.getItem(LS_FROM) ? dayjs(localStorage.getItem(LS_FROM)!) : undefined,
  );
  const [customTo,   setCustomTo]   = useState<Dayjs | undefined>(
    localStorage.getItem(LS_TO) ? dayjs(localStorage.getItem(LS_TO)!) : undefined,
  );
  const [topCount, setTopCount] = useState<number>(Number(localStorage.getItem(LS_TOP)) || 5);

  const [data,    setData]    = useState<DashboardDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error,   setError]   = useState<string | null>(null);

  const load = useCallback(() => {
    const [from, to] = resolveDates(preset, customFrom, customTo);
    setLoading(true);
    setError(null);
    getDashboard(from.toISOString(), to.toISOString(), topCount)
      .then(res => setData(res.data.data!))
      .catch(e => setError(e?.response?.data?.message ?? e?.message ?? 'Ошибка загрузки данных'))
      .finally(() => setLoading(false));
  }, [preset, customFrom, customTo, topCount]);

  useEffect(() => { load(); }, [load]);

  const handlePreset = (value: string | number) => {
    const p = value as Preset;
    setPreset(p);
    localStorage.setItem(LS_PRESET, p);
  };

  const handleRange = (dates: [Dayjs | null, Dayjs | null] | null) => {
    if (dates?.[0] && dates?.[1]) {
      setCustomFrom(dates[0]);
      setCustomTo(dates[1]);
      localStorage.setItem(LS_FROM, dates[0].toISOString());
      localStorage.setItem(LS_TO,   dates[1].toISOString());
    }
  };

  const handleTopCount = (value: number) => {
    setTopCount(value);
    localStorage.setItem(LS_TOP, String(value));
  };

  const [fromDate, toDate] = resolveDates(preset, customFrom, customTo);
  const periodLabel = `${fromDate.format('DD.MM.YYYY')} — ${toDate.format('DD.MM.YYYY')}`;

  if (loading) return <Spin fullscreen />;
  if (error)   return <Alert type="error" message="Ошибка" description={error} showIcon style={{ maxWidth: 500 }} />;
  if (!data)   return null;

  return (
    <>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20, flexWrap: 'wrap', gap: 12 }}>
        <Typography.Title level={4} style={{ margin: 0 }}>Главная</Typography.Title>
        <Space wrap>
          <Segmented
            options={[
              { label: 'Неделя', value: 'week'   },
              { label: 'Месяц',  value: 'month'  },
              { label: 'Год',    value: 'year'   },
              { label: 'Период', value: 'custom' },
            ]}
            value={preset}
            onChange={handlePreset}
          />
          {preset === 'custom' && (
            <RangePicker
              format="DD.MM.YYYY"
              value={[customFrom ?? null, customTo ?? null]}
              onChange={handleRange}
            />
          )}
          <Select
            value={topCount}
            onChange={handleTopCount}
            options={[
              { label: 'Топ-3',  value: 3  },
              { label: 'Топ-5',  value: 5  },
              { label: 'Топ-10', value: 10 },
            ]}
            style={{ width: 90 }}
          />
        </Space>
      </div>

      <Row gutter={[16, 16]} style={{ marginBottom: 16 }}>
        <Col xs={24} sm={8}>
          <Card>
            <Statistic title="Принято" value={data.received}
              prefix={<ClockCircleOutlined />} valueStyle={{ color: '#1677ff' }} />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card>
            <Statistic title="В работе" value={data.inProgress}
              prefix={<SyncOutlined spin />} valueStyle={{ color: '#fa8c16' }} />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card>
            <Statistic title="Завершено (не выдано)" value={data.completed}
              prefix={<CheckCircleOutlined />} valueStyle={{ color: '#13c2c2' }} />
          </Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col xs={24} sm={12}>
          <Card>
            <Statistic title={`Ремонтов за период`} value={data.repairsForPeriod}
              prefix={<CalendarOutlined />}
              suffix={<Typography.Text type="secondary" style={{ fontSize: 12 }}>{periodLabel}</Typography.Text>}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12}>
          <Card>
            <Statistic title={`Выручка за период`} value={data.revenueForPeriod}
              precision={2} suffix="₽" prefix={<DollarOutlined />}
              valueStyle={{ color: '#52c41a' }}
            />
          </Card>
        </Col>
      </Row>

      <Card title={`Топ-${topCount} исполнителей · ${periodLabel}`}>
        <Table
          dataSource={data.topExecutors}
          columns={executorColumns}
          rowKey="name"
          size="small"
          pagination={false}
          showSorterTooltip={false}
          locale={{ emptyText: 'Нет данных за выбранный период' }}
        />
      </Card>
    </>
  );
}
