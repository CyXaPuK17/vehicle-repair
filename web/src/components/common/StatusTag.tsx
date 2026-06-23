import { Tag } from 'antd';

interface Props {
  isActive: boolean;
}

export default function StatusTag({ isActive }: Props) {
  return <Tag color={isActive ? 'green' : 'red'}>{isActive ? 'Активен' : 'Неактивен'}</Tag>;
}
