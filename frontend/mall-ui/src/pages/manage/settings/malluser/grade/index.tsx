import { PlusOutlined } from '@ant-design/icons';
import { Button, Card, Form, Input, message, Modal, Table } from 'antd';
import React, { useEffect, useState } from 'react';

import http from '@/utils/http';
import { ColumnProps } from 'antd/es/table';

export default (props: any) => {
  const [loading, _loading] = useState(false);
  const [loadingSave, _loadingSave] = useState(false);
  const [loadingId, _loadingId] = useState(0);
  const [datalist, _datalist] = useState<any[]>([]);

  const [show, _show] = useState(false);
  const [formData, _formData] = useState({});

  const [form] = Form.useForm();

  useEffect(() => {
    if (formData) {
      form.setFieldsValue(formData);
    }
  }, [formData]);

  const queryList = () => {
    _loading(true);

    http.apiRequest
      .post('/mall-admin/user-grade/list', {})
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _datalist(res.data.Data || []);
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const save = (formData: any) => {
    _loadingSave(true);

    http.apiRequest
      .post('/mall-admin/user-grade/save', {
        ...formData,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('保存成功');
          _show(false);
          queryList();
        }
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  const deletePrice = (row: any) => {
    if (!confirm('确定删除？')) {
      return;
    }
    var id: number = row.Id;
    _loadingId(id);
    http.apiRequest
      .post('/mall-admin/user-grade/update-status', {
        Id: id,
        IsDeleted: true,
      })
      .then((res) => {
        if (res.data.Error) {
          alert(res.data.Error.Message);
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
      title: '会员名',
      render: (x) => x.Name,
    },
    {
      title: '会员描述',
      render: (x) => x.Description,
    },
    {
      title: '会员人数',
      render: (x) => x.UserCount,
    },
    {
      title: '操作',
      width: 200,
      render: (x) => {
        return (
          <Button.Group>
            <Button
              onClick={() => {
                _formData(x);
                _show(true);
              }}
            >
              编辑
            </Button>
            <Button
              loading={loadingId === x.Id}
              onClick={() => {
                deletePrice(x);
              }}
              danger
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
  }, []);

  useEffect(() => {
    form.resetFields();
    if (formData) {
      form.setFieldsValue(formData);
    }
  }, [formData]);

  return (
    <>
      <Modal
        title="编辑价格"
        open={show}
        onCancel={() => {
          _show(false);
        }}
        confirmLoading={loadingSave}
        onOk={() => {
          form.submit();
        }}
      >
        <Form
          form={form}
          onFinish={(e) => {
            save(e);
          }}
        >
          <Form.Item hidden name="Id" label="名称">
            <Input />
          </Form.Item>
          <Form.Item name="Name" label="名称">
            <Input />
          </Form.Item>
          <Form.Item name="Description" label="描述">
            <Input />
          </Form.Item>
        </Form>
      </Modal>
      <Card
        title="商品全局会员价格"
        size="small"
        loading={loading}
        style={{
          marginBottom: 10,
        }}
        extra={
          <Button
            type="primary"
            size="small"
            icon={<PlusOutlined />}
            onClick={() => {
              _show(true);
              _formData({});
            }}
          >
            新增
          </Button>
        }
      >
        <Table
          size="small"
          dataSource={datalist}
          columns={columns}
          pagination={false}
        />
      </Card>
    </>
  );
};
