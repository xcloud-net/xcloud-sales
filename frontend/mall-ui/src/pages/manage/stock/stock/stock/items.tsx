import { StockItemDto } from '@/utils/models';
import { Table } from 'antd';
import { ColumnProps } from 'antd/es/table';

export default ({ data }: { data: StockItemDto[] }) => {
  const columns: ColumnProps<any>[] = [
    {
      title: '商品',
      render: (x: StockItemDto) => {
        return (
          <span>{`${x.Goods?.Name || '--'}/${
            x.Combination?.Name || '--'
          }`}</span>
        );
      },
    },
    {
      title: '采购单价',
      render: (x: StockItemDto) => {
        return <span>{x.Price}</span>;
      },
    },
    {
      title: '采购数量',
      render: (x: StockItemDto) => {
        return <span>{x.Quantity}</span>;
      },
    },
  ];

  return (
    <>
      <Table
        size="small"
        rowKey={(x) => x.Id}
        columns={columns}
        dataSource={data || []}
        pagination={false}
      />
    </>
  );
};
