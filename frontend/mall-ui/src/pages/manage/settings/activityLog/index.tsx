import XUserAvatar from '@/components/manage/user/avatar';
import u from '@/utils';
import http from '@/utils/http';
import { ActivityLogDto } from '@/utils/models';
import { DeleteFilled } from '@ant-design/icons';
import { Button, Card, message, Table } from 'antd';
import { ColumnProps } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import XDetail from './detail';
import XSearchForm from './searchForm';
import utils from './utils';
import XTime from '@/components/manage/time';

export default (props: any): React.ReactNode => {
  const [loading, _loading] = useState(true);
  const [data, _data] = useState({
    Items: [],
    TotalCount: 0,
  });
  const [query, _query] = useState({
    Page: 1,
  });

  const [loadingId, _loadingId] = useState(0);

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/common/log-paging', {
        ...query,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data || {});
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  const deleteRow = (row: any) => {
    if (!confirm('删除？')) {
      return;
    }
    _loadingId(row.Id);
    http.apiRequest
      .post('/mall-admin/common/delete-log', {
        Id: row.Id,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('删除成功');
          queryList();
        }
      })
      .finally(() => {
        _loadingId(0);
      });
  };

  const columns: ColumnProps<any>[] = [
    {
      title: '日志内容',
      render: (x) => x.Comment || '--',
    },
    {
      title: '日志类型',
      render: (x) => {
        var desc = u.find(
          utils.ActivityLogTypesDescription,
          (d) => d.id == x.ActivityLogTypeId,
        );
        if (!desc) {
          return '--';
        }
        return <span>{desc.name || '--'}</span>;
      },
    },
    {
      title: '管理员',
      render: (x: ActivityLogDto) => {
        return <XUserAvatar model={x.Admin?.SysUser} empty={<span>--</span>} />;
      },
    },
    {
      title: '用户',
      render: (x: ActivityLogDto) => {
        return <XUserAvatar model={x.User?.SysUser} empty={<span>--</span>} />;
      },
    },
    {
      title: '地址',
      render: (x: ActivityLogDto) => x.GeoCity || x.GeoCountry || '--',
    },
    {
      title: '时间',
      render: (x) => <XTime model={x} />,
    },
    {
      fixed: 'right',
      title: '操作',
      width: 200,
      render: (text, record) => {
        return (
          <Button.Group size="small">
            <Button
              icon={<DeleteFilled />}
              loading={loadingId == record.Id}
              type="primary"
              danger
              onClick={() => {
                deleteRow(record);
              }}
            >
              删除
            </Button>
          </Button.Group>
        );
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
      <Card title="活动日志" size="small">
        <Table
          style={{ width: '100%' }}
          size="small"
          rowKey={(x) => x.Id}
          loading={loading}
          columns={columns}
          dataSource={data.Items || []}
          expandable={{
            expandedRowRender: (e) => <XDetail model={e} />,
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
