import http from '@/utils/http';
import { DeleteFilled, EditFilled } from '@ant-design/icons';
import { Button, Card, message, Switch, Table } from 'antd';
import { ColumnProps } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import BrandForm from './form';
import XSearchForm from './searchForm';
import XPicture from './picture';
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
      .post('/mall-admin/brand/paging', {
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
      .post('/mall-admin/brand/update-status', {
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
      render: (x) => x.MetaKeywords || '--',
    },
    {
      title: '图片',
      render: (text, x) => {
        return (
          <>
            <XPicture
              data={x}
              ok={() => {
                queryList();
              }}
            />
          </>
        );
      },
    },
    {
      title: '首页展示',
      render: (text, record) => {
        return (
          <Switch
            checked={record.ShowOnPublicPage}
            loading={updateStatusLoading == record.Id}
            onChange={(e) => {
              console.log(e);
              if (!confirm('确定修改？')) {
                return;
              }
              _updateStatusLoading(record.Id);
              http.apiRequest
                .post('/mall-admin/brand/update-status', {
                  BrandId: record.Id,
                  ShowOnPublicPage: e,
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
        title="品牌"
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
