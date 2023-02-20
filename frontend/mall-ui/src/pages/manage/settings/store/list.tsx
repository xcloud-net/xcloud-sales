import http from '@/utils/http';
import { Button, Card, message, Table, Switch } from 'antd';
import { ColumnType } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import StoreForm from './form';
import XTime from '@/components/manage/time';

export default (props: any): React.ReactNode => {
  const [loading, _loading] = useState(true);
  const [stores, _stores] = useState([]);

  const [showForm, _showForm] = useState(false);
  const [formData, _formData] = useState({});

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/store/list', {})
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          var data = res.data.Data || [];
          _stores(data);
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const columns: ColumnType<any>[] = [
    {
      title: '店铺名称',
      dataIndex: 'StoreName',
    },
    {
      title: '营业状态',
      render: (x) => <Switch checked={x.StoreClosed || false} />,
    },
    {
      title: '时间',
      render: (x) => <XTime model={x} />,
    },
    {
      title: '操作',
      width: '200px',
      render: (text, record) => {
        return (
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
            <Button type="default">修改状态</Button>
          </Button.Group>
        );
      },
    },
  ];

  useEffect(() => {
    queryList();
  }, []);

  return (
    <>
      <StoreForm
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
        extra={
          <Button
            size="small"
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
          dataSource={stores}
          pagination={false}
        />
      </Card>
    </>
  );
};
