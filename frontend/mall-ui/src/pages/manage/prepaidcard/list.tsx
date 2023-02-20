import http from '@/utils/http';
import { DeleteFilled, EditFilled } from '@ant-design/icons';
import { Button, Card, message, Switch, Table, Avatar } from 'antd';
import { ColumnProps } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import XForm from './form';
import XSearchForm from './searchForm';
import XCard from './card';
import u from '@/utils';
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

  const [showForm, _showForm] = useState(false);
  const [formData, _formData] = useState({});
  const [updateStatusLoading, _updateStatusLoading] = useState(0);

  const [loadingId, _loadingId] = useState(0);

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/prepaid-card/paging', {
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

  const deleteRow = (row: any) => {
    if (!confirm('删除充值卡？')) {
      return;
    }
    _loadingId(row.Id);
    http.apiRequest
      .post('/mall-admin/prepaid-card/update-status', {
        Id: row.Id,
        IsDeleted: true,
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
      title: '金额',
      render: (x) => x.Amount,
    },
    {
      title: '截至日期',
      render: (x) =>
        (u.isEmpty(x.EndTime) && u.formatDateTime(x.EndTime)) || '--',
    },
    {
      title: '使用人',
      render: (x) => {
        return <Avatar>{x.UserId}</Avatar>;
      },
    },
    {
      title: '是否可用',
      render: (text, record) => {
        return (
          <Switch
            checked={record.IsActive}
            loading={updateStatusLoading == record.Id}
            onChange={(e) => {
              console.log(e);
              if (!confirm('确定修改？')) {
                return;
              }
              _updateStatusLoading(record.Id);
              http.apiRequest
                .post('/mall-admin/prepaid-card/update-status', {
                  Id: record.Id,
                  IsActive: e,
                })
                .then((res) => {
                  if (res.data.Error) {
                    message.error(res.data.Error.Message);
                  } else {
                    message.success('修改成功');
                    queryList();
                  }
                })
                .finally(() => {
                  _updateStatusLoading(0);
                });
            }}
          />
        );
      },
    },
    {
      title: '时间',
      render: (x) => <XTime model={x} />,
    },
    {
      title: '操作',
      width: 200,
      render: (text, record) => {
        return (
          <Button.Group size="small">
            <Button
              icon={<EditFilled />}
              disabled
              type="primary"
              onClick={() => {
                _formData(record);
                _showForm(true);
              }}
            >
              编辑
            </Button>
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
      <XForm
        show={showForm}
        hide={() => _showForm(false)}
        data={formData}
        ok={() => {
          _showForm(false);
          queryList();
        }}
      />
      <XSearchForm
        query={query}
        onSearch={(q: any) => {
          _query(q);
        }}
      />
      <Card
        title="充值卡"
        size="small"
        extra={
          <Button
            type="primary"
            onClick={() => {
              _formData({});
              _showForm(true);
            }}
          >
            新增
          </Button>
        }
      >
        <Table
          size="small"
          rowKey={(x) => x.Id}
          loading={loading}
          columns={columns}
          dataSource={data.Items || []}
          expandable={{
            expandedRowRender: (x) => <XCard model={x} />,
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
