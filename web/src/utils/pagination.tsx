import { Select } from 'antd';
import type { SelectProps } from 'antd';
import type { TablePaginationConfig } from 'antd';

function PaginationSelect(props: SelectProps) {
  return <Select {...props} showSearch={false} />;
}

export const PAGINATION: TablePaginationConfig = {
  defaultPageSize: 25,
  showSizeChanger: true,
  pageSizeOptions: ['10', '25', '50'],
  locale: { items_per_page: '' },
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  selectComponentClass: PaginationSelect as any,
};
