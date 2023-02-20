import http from '@/utils/http';
import { SupplierDto } from '@/utils/models';
import { DeleteFilled, EditFilled } from '@ant-design/icons';
import { Button, Card, Table, message } from 'antd';
import { ColumnProps } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import BrandForm from './form';
import XSearchForm from './searchForm';

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
  const [loadingId, _loadingId] = useState('');

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/supplier/paging', {
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

  const deleteBrand = (row: any) => {
    if (!confirm('删除品牌？')) {
      return;
    }
    _loadingId(row.Id);
    http.apiRequest
      .post('/mall-admin/supplier/update-status', {
        BrandId: row.Id,
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
        _loadingId('');
      });
  };

  const columns: ColumnProps<any>[] = [
    {
      title: '名称',
      render: (x) => x.Name,
    },
    {
      title: '联系人',
      render: (x) => x.ContactName || '--',
    },
    {
      title: '联系电话',
      render: (x) => x.Telephone || '--',
    },
    {
      title: '地址',
      render: (x) => x.Address || '--',
    },
    {
      title: '操作',
      width: 200,
      render: (record: SupplierDto) => {
        return (
          <Button.Group size="small">
            <Button
              icon={<EditFilled />}
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
      <BrandForm
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
        title="供应商"
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
