import u from '@/utils';
import { Image, Table } from 'antd';
import { ColumnType } from 'antd/es/table';
import { Box } from '@mui/material';
import { GoodsDto } from '@/utils/models';

export default (props: any) => {
  const { items } = props;

  const columns: ColumnType<any>[] = [
    {
      title: '商品图片',
      render: (x) => {
        const goods: GoodsDto = x.Goods;
        const pic = u.first(goods.XPictures || []);
        return (
          pic && (
            <Image
              src={
                u.resolveUrlv2(pic, {
                  width: 200,
                  height: 300,
                }) as string
              }
              width={100}
            />
          )
        );
      },
    },
    {
      title: '商品规格',
      render: (x) => {
        const { Goods, GoodsSpecCombination } = x;
        return `${Goods?.Name}/${GoodsSpecCombination?.Name}`;
      },
    },
    {
      title: '单价',
      render: (x) => x.UnitPrice,
    },
    {
      title: '数量',
      render: (x) => x.Quantity,
    },
    {
      title: '总计',
      render: (x) => x.Price,
    },
  ];

  return (
    <>
      <Box sx={{}}>
        <Table
          size="small"
          rowKey={(x) => x.Id}
          columns={columns}
          dataSource={items}
          pagination={false}
        />
      </Box>
    </>
  );
};
