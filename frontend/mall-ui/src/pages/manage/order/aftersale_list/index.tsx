import XTime from '@/components/manage/time';
import XUserAvatar from '@/components/manage/user/avatar';
import XAftersaleStatusTag from '@/components/status/order/aftersale';
import u from '@/utils';
import { AfterSaleDto } from '@/utils/models';
import { Button, Card, message, Modal, Table } from 'antd';
import { ColumnType } from 'antd/es/table';
import { useEffect, useState } from 'react';
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
      .post('/mall-admin/aftersale/paging', { ...query })
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
      width: 200,
      render: (x: AfterSaleDto) => (
        <span>
          <b>{x.Order?.OrderSn || '--'}</b>
        </span>
      ),
    },
    {
      title: '售后状态',
      render: (x) => {
        return <XAftersaleStatusTag model={x} />;
      },
    },
    {
      title: '售后诉求',
      render: (x) => x.RequestedAction || '--',
    },
    {
      title: '退货理由',
      render: (x) => x.ReasonForReturn || '--',
    },
    {
      title: '备注',
      render: (x) => x.UserComments || '--',
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
      render: (x: AfterSaleDto) => {
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
      render: (x: AfterSaleDto) => (
        <Button.Group>
          <Button
            type="dashed"
            onClick={() => {
              _detailId(x.OrderId || '');
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
