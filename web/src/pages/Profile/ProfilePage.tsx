import { useEffect, useState } from 'react';
import { Card, Form, Input, Button, Descriptions, Typography, Spin, message, Divider } from 'antd';
import { UserOutlined, EditOutlined, SaveOutlined, CloseOutlined } from '@ant-design/icons';
import { getMyProfile, updateMyProfile } from '../../api/users';
import type { ProfileDto } from '../../types';
import dayjs from 'dayjs';

const roleLabel: Record<string, string> = {
  ManagementCompany: 'Управляющая компания',
  Executor: 'Исполнитель',
  Customer: 'Заказчик',
};

export default function ProfilePage() {
  const [profile, setProfile] = useState<ProfileDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [form] = Form.useForm();

  const hasLinkedEntity = profile?.name !== undefined && profile?.name !== null;

  const load = async () => {
    try {
      const res = await getMyProfile();
      setProfile(res.data.data!);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const startEdit = () => {
    form.setFieldsValue({
      name: profile?.name,
      contactPerson: profile?.contactPerson,
      address: profile?.address,
      phone: profile?.phone,
      email: profile?.email,
    });
    setEditing(true);
  };

  const cancelEdit = () => {
    setEditing(false);
    form.resetFields();
  };

  const handleSave = async () => {
    const values = await form.validateFields();
    setSaving(true);
    try {
      await updateMyProfile(values);
      message.success('Профиль обновлён');
      setEditing(false);
      await load();
    } catch {
      message.error('Ошибка при сохранении');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return <Spin style={{ display: 'block', marginTop: 80 }} />;
  }

  return (
    <div style={{ maxWidth: 640 }}>
      <Typography.Title level={4} style={{ marginBottom: 24 }}>
        <UserOutlined style={{ marginRight: 8 }} />
        Профиль
      </Typography.Title>

      <Card>
        <Descriptions column={1} size="middle">
          <Descriptions.Item label="Логин">{profile?.login}</Descriptions.Item>
          <Descriptions.Item label="Роль">{profile ? roleLabel[profile.role] : '—'}</Descriptions.Item>
          {profile?.inn && (
            <Descriptions.Item label="ИНН">{profile.inn}</Descriptions.Item>
          )}
          <Descriptions.Item label="Дата регистрации">
            {profile?.createdAt ? dayjs(profile.createdAt).format('DD.MM.YYYY') : '—'}
          </Descriptions.Item>
          {profile?.lastLoginAt && (
            <Descriptions.Item label="Последний вход">
              {dayjs(profile.lastLoginAt).format('DD.MM.YYYY HH:mm')}
            </Descriptions.Item>
          )}
        </Descriptions>

        {hasLinkedEntity && (
          <>
            <Divider />
            {editing ? (
              <Form form={form} layout="vertical">
                <Form.Item name="name" label="Наименование" rules={[{ required: true, message: 'Укажите наименование' }]}>
                  <Input />
                </Form.Item>
                {profile?.role === 'Customer' && (
                  <Form.Item name="contactPerson" label="Контактное лицо">
                    <Input />
                  </Form.Item>
                )}
                {profile?.role === 'Executor' && (
                  <Form.Item name="address" label="Адрес">
                    <Input />
                  </Form.Item>
                )}
                <Form.Item name="phone" label="Телефон">
                  <Input />
                </Form.Item>
                <Form.Item name="email" label="Email" rules={[{ type: 'email', message: 'Некорректный email' }]}>
                  <Input />
                </Form.Item>
                <Form.Item style={{ marginBottom: 0 }}>
                  <Button
                    type="primary"
                    icon={<SaveOutlined />}
                    onClick={handleSave}
                    loading={saving}
                    style={{ marginRight: 8 }}
                  >
                    Сохранить
                  </Button>
                  <Button icon={<CloseOutlined />} onClick={cancelEdit}>
                    Отмена
                  </Button>
                </Form.Item>
              </Form>
            ) : (
              <>
                <Descriptions column={1} size="middle">
                  <Descriptions.Item label="Наименование">{profile?.name || '—'}</Descriptions.Item>
                  {profile?.role === 'Customer' && (
                    <Descriptions.Item label="Контактное лицо">{profile?.contactPerson || '—'}</Descriptions.Item>
                  )}
                  {profile?.role === 'Executor' && (
                    <Descriptions.Item label="Адрес">{profile?.address || '—'}</Descriptions.Item>
                  )}
                  <Descriptions.Item label="Телефон">{profile?.phone || '—'}</Descriptions.Item>
                  <Descriptions.Item label="Email">{profile?.email || '—'}</Descriptions.Item>
                </Descriptions>
                <Button
                  icon={<EditOutlined />}
                  onClick={startEdit}
                  style={{ marginTop: 16 }}
                >
                  Редактировать
                </Button>
              </>
            )}
          </>
        )}
      </Card>
    </div>
  );
}
