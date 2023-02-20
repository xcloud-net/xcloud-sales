import XTime from '@/components/manage/time';
import http from '@/utils/http';
import { OrderBillDto, PagedResponse } from '@/utils/models';
import { Card, Table, Tag, message } from 'antd';
import { ColumnProps } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import XSearchForm from './searchForm';
import XDetail from './detail';
import utils from '@/utils/order';

export default (props: any): React.ReactNode => {
  const [loading, _loading] = useState(true);
  const [data, _data] = useState<PagedResponse<OrderBillDto>>({
    Items: [],
    TotalCount: 0,
  });
  const [query, _query] = useState({
    Page: 1,
  });

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/order-bill/paging', {
        ...query,
      })
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

  const columns: ColumnProps<OrderBillDto>[] = [
    {
      title: '支付单号',
      width: 200,
      render: (x: OrderBillDto) => {
        return (
          <span>
            <b>{x.Id}</b>
          </span>
        );
      },
    },
    {
      title: '订单号',
      render: (x: OrderBillDto) => {
        return <span>{x.Order?.OrderSn || '--'}</span>;
      },
    },
    {
      title: '支付方式',
      render: (x: OrderBillDto) => {
        var paymentMethod = utils.getPaymentMethod(x.PaymentMethod);
        if (paymentMethod) {
          return (
            <span>
              <b>{paymentMethod.name || '--'}</b>
            </span>
          );
        }
        return <span>{x.PaymentMethod}</span>;
      },
    },
    {
      title: '支付状态',
      render: (x: OrderBillDto) => {
        return x.Paid && <Tag color="green">已支付</Tag>;
      },
    },
    {
      title: '退款状态',
      render: (x: OrderBillDto) => {
        return x.Refunded && <Tag color="red">已退款</Tag>;
      },
    },
    {
      title: '时间',
      render: (x: OrderBillDto) => {
        return <XTime model={x} />;
      },
    },
  ];

  useEffect(() => {
    queryList();
  }, [query]);

  return (
    <>
      <XSearchForm
        query={query}
        onSearch={(q: any) => {
          _query(q);
        }}
      />
      <Card title="库存明细" size="small">
        <Table
          size="small"
          rowKey={(x) => x.Id || ''}
          loading={loading}
          columns={columns}
          dataSource={data.Items || []}
          expandable={{
            expandedRowRender: (x) => <XDetail model={x} />,
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
