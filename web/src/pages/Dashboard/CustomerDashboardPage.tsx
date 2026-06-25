import { useEffect, useState } from 'react';
import { Card, Col, Row, Statistic, Typography, Spin, Alert } from 'antd';
import { CarOutlined, SyncOutlined, DollarOutlined } from '@ant-design/icons';
import { getCustomerDashboard, type CustomerDashboardDto } from '../../api/dashboard';
import dayjs from 'dayjs';

export default function CustomerDashboardPage() {
  const [data, setData] = useState<CustomerDashboardDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getCustomerDashboard()
      .then(res => setData(res.data.data!))
      .catch(e => setError(e?.response?.data?.message ?? e?.message ?? 'Ошибка загрузки данных'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <Spin fullscreen />;
  if (error)   return <Alert type="error" message="Ошибка" description={error} showIcon style={{ maxWidth: 500 }} />;
  if (!data)   return null;

  const year = dayjs().year();

  return (
    <>
      <Typography.Title level={4} style={{ marginBottom: 20 }}>Главная</Typography.Title>
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={8}>
          <Card>
            <Statistic
              title="Мои ТС"
              value={data.vehicleCount}
              prefix={<CarOutlined />}
              valueStyle={{ color: '#1677ff' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card>
            <Statistic
              title="Активных ремонтов"
              value={data.activeRepairs}
              prefix={<SyncOutlined spin={data.activeRepairs > 0} />}
              valueStyle={{ color: '#fa8c16' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card>
            <Statistic
              title={`Потрачено за ${year} год`}
              value={data.spentForYear}
              precision={2}
              suffix="₽"
              prefix={<DollarOutlined />}
              valueStyle={{ color: '#52c41a' }}
            />
          </Card>
        </Col>
      </Row>
    </>
  );
}
