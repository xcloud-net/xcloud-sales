import u from '@/utils';
import { Table, Card } from 'antd';
import { ColumnProps } from 'antd/es/table';
import React, { useEffect } from 'react';
import utils from '@/utils/order';

const App = (props: any) => {
  const { model } = props;

  const [data, _data] = React.useState<any>([]);
  const [loading, _loading] = React.useState(false);

  const queryBill = () => {
    if (u.isEmpty(model.Id)) {
      return;
    }
    _loading(true);
    u.http.apiRequest
      .post('/mall-admin/order/list-order-bill', { Id: model.Id })
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data.Data || []);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryBill();
  }, [model]);

  const columns: ColumnProps<any>[] = [
    {
      title: '账单',
      render: (x) => x.Id,
    },
    {
      title: '金额',
      render: (x) => x.Price,
    },
    {
      title: '支付方式',
      render: (x) => {
        var paymentMethod = u.find(
          utils.allStatus.paymentMethod,
          (d) => d.id == x.PaymentMethod,
        );
        return paymentMethod?.name || '--';
      },
    },
    {
      title: '支付状态',
      render: (x) => (x.Paid ? '已支付' : '未支付'),
    },
    {
      title: '支付时间',
      render: (x) => u.dateTimeFromNow(x.PayTime),
    },
    {
      title: 'out_trade_no',
      render: (x) => x.PaymentTransactionId || '--',
    },
    {
      title: '创建时间',
      render: (x) => u.dateTimeFromNow(x.CreationTime),
    },
  ];

  return (
    <>
      <Card title="支付账单" size="small">
        <Table
          size="small"
          loading={loading}
          dataSource={data}
          columns={columns}
          rowKey={(x) => x.Id}
          pagination={false}
        />
      </Card>
    </>
  );
};

export default App;
