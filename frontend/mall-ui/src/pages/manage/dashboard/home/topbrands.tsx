import u from '@/utils';
import { Button, Card, Table } from 'antd';
import { ColumnProps } from 'antd/es/table';
import { useEffect, useState } from 'react';
import XIndexNo from '../components/indexNo';

const columns: ColumnProps<any>[] = [
  {
    render: (x, record, index) => <XIndexNo index={index} />,
  },
  {
    title: '品牌',
    render: (x) => x.BrandName,
  },
  {
    title: '金额',
    render: (x) => x.TotalPrice,
  },
  {
    title: '数量',
    render: (x) => x.TotalQuantity,
  },
];

const App = (props: any) => {
  const [data, _data] = useState<any>([]);
  const [loading, _loading] = useState(false);

  const [finalQuery, _finalQuery] = useState<any>({
    StartTime: null,
    EndTime: null,
    MaxCount: null,
  });

  const queryTop = (q: any) => {
    _loading(true);
    u.http.apiRequest
      .post('/mall-admin/report/top-brands', {
        ...q,
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
    queryTop({ ...finalQuery });
  }, []);

  useEffect(() => {
    var now = u.dayjs.utc();
    var monthAgo = now.add(-1, 'month');
    _finalQuery({
      StartTime: monthAgo.format(u.timeFormat),
      EndTime: now.format(u.timeFormat),
      first: false,
    });
  }, []);

  return (
    <>
      <Card
        title="TOP品牌"
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
