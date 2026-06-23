import { DatePicker, Button, Space, Form } from 'antd';
import { DownloadOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';

const { RangePicker } = DatePicker;

interface Props {
  onSearch: (from: string, to: string) => void;
  onExport?: (format: 'xlsx' | 'pdf') => void;
  loading?: boolean;
}

export default function ReportFilter({ onSearch, onExport, loading }: Props) {
  const [form] = Form.useForm();

  const handleSearch = () => {
    const { range } = form.getFieldsValue();
    if (range?.length === 2) {
      onSearch(
        (range[0] as dayjs.Dayjs).startOf('day').toISOString(),
        (range[1] as dayjs.Dayjs).endOf('day').toISOString()
      );
    }
  };

  return (
    <Form form={form} layout="inline" style={{ marginBottom: 16 }}>
      <Form.Item name="range" initialValue={[dayjs().subtract(30, 'day'), dayjs()]}>
        <RangePicker format="DD.MM.YYYY" />
      </Form.Item>
      <Form.Item>
        <Button type="primary" onClick={handleSearch} loading={loading}>
          Сформировать
        </Button>
      </Form.Item>
      {onExport && (
        <Form.Item>
          <Space>
            <Button icon={<DownloadOutlined />} onClick={() => onExport('xlsx')}>
              Excel
            </Button>
            <Button icon={<DownloadOutlined />} onClick={() => onExport('pdf')}>
              PDF
            </Button>
          </Space>
        </Form.Item>
      )}
    </Form>
  );
}
