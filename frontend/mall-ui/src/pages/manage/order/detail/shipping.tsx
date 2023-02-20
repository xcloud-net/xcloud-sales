import u from '@/utils';
import { Table, Card } from 'antd';
import { ColumnProps } from 'antd/es/table';
import React, { useEffect } from 'react';

const App = (props: any) => {
  const { model } = props;

  const [data, _data] = React.useState<any>([]);
  const [loading, _loading] = React.useState(false);

  const queryList = () => {
    if (u.isEmpty(model.Id)) {
      return;
    }
    _loading(true);
    u.http.apiRequest
      .post('/mall-admin/shipping/by-order-id', { Id: model.Id })
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
    queryList();
  }, [model]);

  const columns: ColumnProps<any>[] = [
    {
      title: '配送方式',
      render: (x) => x.ShippingMethod,
    },
    {
      title: '物流名称',
      render: (x) => x.ExpressName,
    },
    {
      title: '物流单号',
      render: (x) => x.TrackingNumber,
    },
    {
      title: '重量',
      render: (x) => x.TotalWeight,
    },
    {
      title: '创建时间',
      render: (x) => u.dateTimeFromNow(x.CreationTime),
    },
  ];

  return (
    <>
      <Card title="发货单" style={{ marginBottom: 10 }} size="small">
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
