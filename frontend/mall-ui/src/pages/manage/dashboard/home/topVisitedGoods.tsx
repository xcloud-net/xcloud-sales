import u from '@/utils';
import { Button, Card, Table } from 'antd';
import { ColumnProps } from 'antd/es/table';
import { useEffect, useState } from 'react';
import XIndexNo from '../components/indexNo';

const App = (props: any) => {
  const [data, _data] = useState<any>([]);
  const [loading, _loading] = useState(false);

  const [finalQuery, _finalQuery] = useState<any>({
    StartTime: null,
    EndTime: null,
  });

  const queryTop = (q: any) => {
    _loading(true);
    u.http.apiRequest
      .post('/mall-admin/report/top-visited-goods', {
        ...q,
        Count: 20,
      })
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
    var now = u.dayjs.utc();
    var monthAgo = now.add(-1, 'month');
    var p = {
      StartTime: monthAgo.format(u.timeFormat),
      EndTime: now.format(u.timeFormat),
      first: false,
    };
    _finalQuery(p);
    queryTop(p);
  }, []);

  const columns: ColumnProps<any>[] = [
    {
      render: (x, record, index) => <XIndexNo index={index} />,
    },
    {
      title: '商品',
      render: (x) => {
        const { Goods } = x;
        return Goods?.Name || '--';
      },
    },
    {
      title: '数量',
      render: (x) => x.VisitedCount,
    },
  ];

  return (
    <>
      <Card
        title="浏览最多的商品"
        extra={<Button type="dashed">更多</Button>}
        loading={loading}
        style={{ marginBottom: 10 }}
      >
        <Table
          size="small"
          columns={columns}
          dataSource={data}
          pagination={false}
        />
      </Card>
    </>
  );
};

export default App;
