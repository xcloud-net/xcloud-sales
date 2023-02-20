import { AfterSalesItemDto } from '@/utils/models';
import { Card, Table } from 'antd';
import { ColumnType } from 'antd/es/table';

export default (props: any) => {
  const { model } = props;

  const columns: ColumnType<any>[] = [
    {
      title: '商品规格',
      render: (x) => {
        const { Goods, GoodsSpecCombination } = x.OrderItem || {};
        return `${Goods?.Name}/${GoodsSpecCombination?.Name}`;
      },
    },
    {
      title: '购入单价',
      render: (x: AfterSalesItemDto) => {
        return x.OrderItem?.UnitPrice || '--';
      },
    },
    {
      title: '售后数量',
      render: (x) => x.Quantity,
    },
  ];

  return (
    <>
      <Card title="商品" style={{ marginBottom: 10 }} size="small">
        <Table
          size="small"
          rowKey={(x) => x.Id}
          columns={columns}
          dataSource={model.Items || []}
          pagination={false}
        />
      </Card>
    </>
  );
};
