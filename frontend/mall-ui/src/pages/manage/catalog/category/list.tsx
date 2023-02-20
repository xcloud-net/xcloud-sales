import u from '@/utils';
import http from '@/utils/http';
import { Button, Card, message, Table } from 'antd';
import { ColumnType } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import BrandForm from './form';
import XStatus from './status';
import XPicture from './picture';
import XTime from '@/components/manage/time';

export default (props: any): React.ReactNode => {
  const [loading, _loading] = useState(true);
  const [data, _data] = useState<any>([]);
  const [expandKeys, _expandKeys] = useState([]);
  const autoExpand = false;

  const [showForm, _showForm] = useState(false);
  const [formData, _formData] = useState({});

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/category/tree', {})
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          var treeData = res.data.Data || [];
          _data(treeData);
          autoExpand && _expandKeys(u.getTreeKeys(treeData));
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const columns: ColumnType<any>[] = [
    {
      title: '名称',
      render: (record: any) => record.title,
    },
    {
      title: 'SeoName',
      render: (x) => x.raw_data?.SeoName || '--',
    },
    {
      title: '描述',
      render: (x) => x.raw_data?.Description,
    },
    {
      title: '图片',
      render: (text, x) => {
        const record = x.raw_data || {};
        return (
          <>
            <XPicture
              data={record}
              ok={() => {
                queryList();
              }}
            />
          </>
        );
      },
    },
    {
      title: '排序',
      render: (x) => x.raw_data?.DisplayOrder,
    },
    {
      title: '状态',
      render: (text, x) => {
        const record = x.raw_data || {};
        return (
          <>
            <XStatus
              data={record}
              ok={() => {
                queryList();
              }}
            />
          </>
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
      render: (text, x) => {
        const record = x.raw_data || {};
        return (
          <>
            <Button.Group size="small">
              <Button
                type="primary"
                onClick={() => {
                  _formData(record);
                  _showForm(true);
                }}
              >
                编辑
              </Button>
              <Button
                type="primary"
                danger
                disabled={!u.isEmpty(x.children)}
                onClick={() => {
                  //
                }}
              >
                删除
              </Button>
            </Button.Group>
          </>
        );
      },
    },
  ];

  useEffect(() => {
    queryList();
  }, []);

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
      <Card
        size="small"
        title="产品分类"
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
          rowKey={(x) => x.key}
          expandable={{
            expandedRowKeys: expandKeys,
            onExpandedRowsChange: (expandedKeys: any) => {
              _expandKeys(expandedKeys);
            },
          }}
          loading={loading}
          columns={columns}
          dataSource={data}
          pagination={false}
        />
      </Card>
    </>
  );
};
