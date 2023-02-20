import XTime from '@/components/manage/time';
import XUserAvatar from '@/components/manage/user/avatar';
import XOrderStatusTag from '@/components/status/order';
import u from '@/utils';
import { OrderDto } from '@/utils/models';
import { Button, Card, message, Modal, Table } from 'antd';
import { ColumnType } from 'antd/es/table';
import { useEffect, useState } from 'react';
import XGoodsList from '../components/goodsList';
import XDetail from '../detail';
import XSearchForm from './searchForm';
import XStatus from './status';

export default (props: any) => {
  const [loading, _loading] = useState(false);
  const [data, _data] = useState({
    Items: [],
    TotalCount: 0,
  });

  const [query, _query] = useState<any>({
    Page: 1,
  });

  const [detailId, _detailId] = useState('');

  const queryList = () => {
    _loading(true);
    u.http.apiRequest
      .post('/mall-admin/order/paging', { ...query })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _data(res.data || {});
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryList();
  }, [query]);

  const columns: ColumnType<any>[] = [
    {
      title: '订单号',
      render: (x) => x.OrderSn,
    },
    {
      title: '总金额',
      render: (x) => x.OrderTotal,
    },
    {
      title: '订单状态',
      render: (x) => {
        return <XOrderStatusTag model={x} />;
      },
    },
    {
      title: '备注',
      render: (x) => x.Remark || '--',
    },
    {
      title: '状态',
      render: (x) => (
        <XStatus
          model={x}
          ok={() => {
            queryList();
          }}
        />
      ),
    },
    {
      title: '买家信息',
      render: (x: OrderDto) => {
        return (
          <>
            <XUserAvatar model={x.User?.SysUser} />
          </>
        );
      },
    },
    {
      title: '时间',
      render: (x) => <XTime model={x} />,
    },
    {
      title: '操作',
      fixed: 'right',
      width: 100,
      render: (x) => (
        <Button.Group>
          <Button
            type="primary"
            onClick={() => {
              _detailId(x.Id);
            }}
          >
            查看
          </Button>
        </Button.Group>
      ),
    },
  ];

  return (
    <>
      <Modal
        open={!u.isEmpty(detailId)}
        onCancel={() => {
          _detailId('');
        }}
        footer={false}
        forceRender
        width="100%"
      >
        <XDetail detailId={detailId} />
      </Modal>
      <XSearchForm
        query={query}
        onSearch={(q: any) => {
          _query(q);
        }}
      />
      <Card bordered={false} loading={loading}>
        <Table
          rowKey={(x) => x.Id}
          loading={loading}
          columns={columns}
          dataSource={data.Items || []}
          expandable={{
            expandedRowRender: (x) => {
              return u.isEmpty(x.Items) || <XGoodsList items={x.Items} />;
            },
          }}
          pagination={{
            showSizeChanger: false,
            pageSize: 20,
            current: query.Page,
            total: data.TotalCount,
            onChange: (e) => {
              _query({
                ...query,
                Page: e,
              });
            },
          }}
        />
      </Card>
    </>
  );
};
