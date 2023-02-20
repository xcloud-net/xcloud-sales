import { Box } from '@mui/material';
import { Table, Tag } from 'antd';
import { ColumnType } from 'antd/es/table';

export default (props: any) => {
  const { items } = props;

  const columns: ColumnType<any>[] = [
    {
      title: 'Sku',
      render: (x) => x.Sku,
    },
    {
      title: '商品规格',
      render: (x) => x.Name,
    },
    {
      title: '单价',
      render: (x) => x.Price,
    },
    {
      title: '数量',
      render: (x) => {
        if (x.StockQuantity <= 0) {
          return <Tag color="red">无库存</Tag>;
        }
        return <p>{x.StockQuantity}</p>;
      },
    },
  ];

  return (
    <>
      <Box
        sx={{
          p: 2,
          m: 2,
          border: '1px solid gray',
          '&:hover': {
            border: '1px solid rgb(0, 127, 255)',
          },
        }}
      >
        <Table
          rowKey={(x) => x.Id}
          columns={columns}
          dataSource={items}
          pagination={false}
        />
      </Box>
    </>
  );
};
