import { Tag } from 'antd';

interface Props {
  active: boolean;
}

export default function InactiveHint({ active }: Props) {
  return active ? null : <Tag color="default" style={{ marginLeft: 6 }}>неактивен</Tag>;
}
