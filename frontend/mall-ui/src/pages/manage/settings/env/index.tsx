import u from '@/utils';
import http from '@/utils/http';
import { Card, Table } from 'antd';
import { ColumnType } from 'antd/es/table';
import { useEffect, useState } from 'react';

export default (props: any) => {
  const [data, _data] = useState<any>([]);
  const [loading, _loading] = useState(false);

  const queryData = () => {
    _loading(true);

    http.apiRequest
      .post('/mall-admin/setting/list', {})
      .then((res) => {
        _data(res.data.Data || []);
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    //queryData();
  }, []);

  const columns: ColumnType<any>[] = [
    {
      title: 'ID',
      render: (x) => x.Id,
    },
    {
      title: '名称',
      render: (x) => x.Name,
    },
    {
      title: '创建时间',
      render: (x) => u.dateTimeFromNow(x.CreationTime) || '--',
    },
    {
      title: '更新时间',
      render: (x) => u.dateTimeFromNow(x.LastModificationTime) || '--',
    },
  ];

  return (
    <>
      <Card
        title="环境变量"
        loading={loading}
        style={{
          marginBottom: 10,
        }}
      >
        <Table
          rowKey={(x) => x.Id}
          columns={columns}
          dataSource={data}
          pagination={false}
          expandable={{
            expandedRowRender: (x) => (
              <pre>
                <code>{x.Value || '--'}</code>
              </pre>
            ),
          }}
        ></Table>
      </Card>
    </>
  );
};
