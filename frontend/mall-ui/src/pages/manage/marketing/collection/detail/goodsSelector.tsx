import u from '@/utils';
import http from '@/utils/http';
import { EditFilled } from '@ant-design/icons';
import { Button, Card, Input, message, Table, Tag } from 'antd';
import { ColumnProps } from 'antd/es/table';
import { useState } from 'react';

export default (props: any) => {
  const { onSelect, onClear, model, selectedGoods } = props;

  const [loading, _loading] = useState(false);
  const [data, _data] = useState<any>([]);
  const [keyword, _keyword] = useState('');

  const queryList = () => {
    if (u.isEmpty(keyword)) {
      message.info('请输入关键字');
      return;
    }

    _loading(true);
    http.apiRequest
      .post('/mall-admin/goods/query-combination-for-selection', {
        Keywords: keyword,
        CollectionId: model.Id,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _data(res.data.Data || []);
        }
      })
      .finally(() => {
        _loading(false);
        onClear && onClear();
      });
  };

  const columns: ColumnProps<any>[] = [
    {
      title: '名称',
      render: (x) => `${x.Goods?.Name}/${x.Name}`,
    },
    {
      title: '描述',
      render: (x) => x.Goods?.ShortDescription,
    },
    {
      title: '价格',
      render: (x) => x.Price,
    },
    {
      render: (x) => {
        return (
          selectedGoods &&
          selectedGoods.Id == x.Id && <Tag color={'green'}>已选择</Tag>
        );
      },
    },
    {
      title: '操作',
      width: 200,
      render: (text, record) => {
        return (
          <Button.Group size="small">
            <Button
              icon={<EditFilled />}
              type="primary"
              onClick={() => {
                onSelect && onSelect(record);
              }}
            >
              选择
            </Button>
          </Button.Group>
        );
      },
    },
  ];

  return (
    <>
      <Card
        bordered={false}
        title="搜索商品规格"
        size="small"
        extra={
          <Input.Search
            size="small"
            placeholder="搜索商品"
            allowClear
            onSearch={() => {
              queryList();
            }}
            onChange={(e) => _keyword(e.target.value)}
          />
        }
      >
        <Table
          size="small"
          rowKey={(x) => x.Id}
          loading={loading}
          columns={columns}
          dataSource={data || []}
          pagination={false}
        />
      </Card>
    </>
  );
};
