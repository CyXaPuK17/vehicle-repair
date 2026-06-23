import { Tag } from 'antd';
import type { RepairStatus } from '../types';

const STATUS_CONFIG: Record<RepairStatus, { label: string; color: string }> = {
  Received:   { label: 'Принят',   color: 'blue' },
  InProgress: { label: 'В работе', color: 'orange' },
  Completed:  { label: 'Завершён', color: 'cyan' },
  Issued:     { label: 'Выдан',    color: 'green' },
};

export const REPAIR_STATUS_OPTIONS: { value: RepairStatus; label: string }[] = [
  { value: 'Received',   label: 'Принят' },
  { value: 'InProgress', label: 'В работе' },
  { value: 'Completed',  label: 'Завершён' },
  { value: 'Issued',     label: 'Выдан' },
];

export function RepairStatusTag({ status }: { status: RepairStatus }) {
  const cfg = STATUS_CONFIG[status] ?? { label: status, color: 'default' };
  return <Tag color={cfg.color}>{cfg.label}</Tag>;
}
