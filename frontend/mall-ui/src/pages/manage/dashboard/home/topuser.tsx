import manageService from '@/services/manage';
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
    MaxCount: null,
  });

  const queryTop = (q: any) => {
    _loading(true);
    u.http.apiRequest
      .post('/mall-admin/report/top-customers', {
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

  const [users, _users] = useState<any>([]);
  useEffect(() => {
    if (u.isEmpty(data)) {
      return;
    }
    var ids = u.map(data, (x) => x.GlobalUserId);
    manageService.querySysUserList(ids).then((res) => {
      _users(res || []);
    });
  }, [data]);

  const columns: ColumnProps<any>[] = [
    {
      render: (x, record, index) => <XIndexNo index={index} />,
    },
    {
      title: '买家',
      render: (x) => {
        var selectedUser = u.find(users, (d) => d.UserId == x.GlobalUserId);
        if (selectedUser) {
          return selectedUser.NickName || '--';
        }
        return '--';
      },
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

  return (
    <>
      <Card
        title="TOP买家"
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
