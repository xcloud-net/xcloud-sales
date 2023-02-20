import http from '@/utils/http';
import { DeleteFilled, EditFilled } from '@ant-design/icons';
import { Button, Card, message, Table } from 'antd';
import { ColumnProps } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import XForm from './form';
import XTime from '@/components/manage/time';
import { PagedResponse, RoleDto } from '@/utils/models';
import u from '@/utils';
import XPermission from './permission';

export default (props: any): React.ReactNode => {
  const [loading, _loading] = useState(true);
  const [data, _data] = useState<PagedResponse<RoleDto>>({
    Items: [],
    TotalCount: 0,
  });
  const [query, _query] = useState({
    Page: 1,
  });

  const [showForm, _showForm] = useState(false);
  const [formData, _formData] = useState<RoleDto>({});
  const [updateStatusLoading, _updateStatusLoading] = useState('');

  const [loadingId, _loadingId] = useState(0);

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/sys/role/paging', {
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

  const deleteBrand = (row: any) => {
    if (!confirm('删除角色？')) {
      return;
    }
    _loadingId(row.Id);
    http.apiRequest
      .post('/sys/role/delete', {
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
      title: 'ID',
      render: (x) => x.Id,
    },
    {
      title: '名称',
      render: (x) => x.Name,
    },
    {
      title: '描述',
      render: (x) => x.Description || '--',
    },
    {
      title: '关键词',
      render: (x) => <XPermission model={x} ok={() => {
        queryList();
      }} />,
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
          <Button.Group size='small'>
            <Button
              icon={<EditFilled />}
              type='primary'
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
              type='primary'
              danger
              onClick={() => {
                deleteBrand(record);
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
      <Card
        title='角色'
        size='small'
        extra={
          <Button
            type='primary'
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
          size='small'
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
