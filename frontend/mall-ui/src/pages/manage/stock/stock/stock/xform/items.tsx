import XCombinationSelect from '@/components/goods/combinationSelect';
import { GoodsCombinationDto, StockItemDto } from '@/utils/models';
import { PlusOutlined } from '@ant-design/icons';
import { Button, InputNumber, Table, message } from 'antd';
import { ColumnType } from 'antd/es/table';
import { useEffect, useState } from 'react';

export default (props: { onChange: any }) => {
  const { onChange } = props;
  const [items, _items] = useState<StockItemDto[]>([]);

  const updateItem = (index: number, updateFunc: any) => {
    if (index > items.length - 1) {
      return;
    }
    var dataCopy = [...items];
    var m = dataCopy[index];
    m = updateFunc(m);
    dataCopy[index] = m;

    //set data
    _items(dataCopy);
  };

  useEffect(() => {
    console.log(items);
    onChange && onChange(items);
  }, [items]);

  const columns: ColumnType<StockItemDto>[] = [
    {
      title: '商品',
      width: 300,
      render: (value: any, record: StockItemDto, index: number) => {
        return (
          <>
            {(record.CombinationId && record.CombinationId > 0) || (
              <div
                style={{
                  fontWeight: 'bold',
                  color: 'red',
                  marginBottom: 5,
                }}
              >
                请选择
              </div>
            )}
            <XCombinationSelect
              selectedCombination={undefined}
              onChange={(e: GoodsCombinationDto, error: any) => {
                if (
                  items.some((m, i) => i != index && m.CombinationId == e.Id)
                ) {
                  error && error('error');
                  message.error('商品已经存在于采购单，请重新选择');
                  return;
                }

                updateItem(
                  index,
                  (d: StockItemDto): StockItemDto => ({
                    ...d,
                    Combination: e,
                    CombinationId: e.Id,
                    GoodsId: e.GoodsId,
                  }),
                );
              }}
              style={{
                width: '100%',
              }}
            />
          </>
        );
      },
    },
    {
      title: '数量',
      render: (value: any, record: StockItemDto, index: number) => {
        return (
          <>
            {(record.Quantity && record.Quantity > 0) || (
              <div
                style={{
                  fontWeight: 'bold',
                  color: 'red',
                  marginBottom: 5,
                }}
              >
                请输入
              </div>
            )}
            <InputNumber
              min={1}
              onChange={(e) => {
                updateItem(
                  index,
                  (d: StockItemDto): StockItemDto => ({
                    ...d,
                    Quantity: e || 0,
                  }),
                );
              }}
            />
          </>
        );
      },
    },
    {
      title: '进货价',
      render: (value: any, record: StockItemDto, index: number) => {
        return (
          <>
            {(record.Price && record.Price > 0) || (
              <div
                style={{
                  fontWeight: 'bold',
                  color: 'red',
                  marginBottom: 5,
                }}
              >
                请输入
              </div>
            )}
            <InputNumber
              min={0}
              onChange={(e) => {
                updateItem(
                  index,
                  (d: StockItemDto): StockItemDto => ({
                    ...d,
                    Price: e || 0,
                  }),
                );
              }}
            />
          </>
        );
      },
    },
    {
      title: '操作',
      render: (value: any, record: StockItemDto, index: number) => {
        return (
          <Button
            type="link"
            onClick={() => {
              if (!confirm('确认删除？')) {
                return;
              }
              var dataCopy = items.map((x) => ({ ...x, remove: false }));
              dataCopy[index].remove = true;

              dataCopy = dataCopy.filter((x) => !x.remove);

              _items(dataCopy);
            }}
          >
            删除
          </Button>
        );
      },
    },
  ];

  return (
    <>
      <Table
        dataSource={items}
        columns={columns}
        pagination={false}
        style={{ marginBottom: 10 }}
      />
      <Button
        icon={<PlusOutlined />}
        block
        type="dashed"
        onClick={() => {
          _items((x) => [...x, {}]);
        }}
      >
        添加商品
      </Button>
    </>
  );
};
