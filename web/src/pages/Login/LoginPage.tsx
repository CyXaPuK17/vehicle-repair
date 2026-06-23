import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Form, Input, Button, Card, Typography, Alert } from 'antd';
import { login } from '../../api/auth';
import { useAuthStore } from '../../store/authStore';

function hasCyrillic(s: string) {
  return /[а-яёА-ЯЁ]/.test(s);
}

export default function LoginPage() {
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { setAuth } = useAuthStore();

  const onFinish = async (values: { login: string; password: string }) => {
    setLoading(true);
    setError('');
    try {
      const res = await login(values.login, values.password);
      const d = res.data.data!;
      setAuth(d.accessToken, d.role, d.userId);
      navigate('/');
    } catch {
      if (hasCyrillic(values.password)) {
        setError('Неверный пароль. Проверьте раскладку клавиатуры — пароль введён кириллицей.');
      } else {
        setError('Неверный логин или пароль.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', background: '#f0f2f5' }}>
      <Card style={{ width: 360 }}>
        <Typography.Title level={3} style={{ textAlign: 'center', marginBottom: 24 }}>
          Учёт ремонтов ТС
        </Typography.Title>
        {error && <Alert type="error" message={error} style={{ marginBottom: 16 }} />}
        <Form layout="vertical" onFinish={onFinish}>
          <Form.Item label="Логин / ИНН" name="login" rules={[{ required: true }]}>
            <Input size="large" placeholder="Введите логин или ИНН" />
          </Form.Item>
          <Form.Item label="Пароль" name="password" rules={[{ required: true }]}>
            <Input.Password size="large" placeholder="Введите пароль" />
          </Form.Item>
          <Button type="primary" htmlType="submit" block size="large" loading={loading}>
            Войти
          </Button>
        </Form>
      </Card>
    </div>
  );
}
