import http from '@/utils/http';
import { Button, Card, Form, Input, message, Modal, Spin, Table } from 'antd';
import { ColumnType } from 'antd/es/table';
import { useEffect, useState } from 'react';
import ValueList from './value';

export default (props: any) => {
  const { model } = props;

  const [loading, _loading] = useState(true);
  const [data, _data] = useState([]);

  const [showForm, _showForm] = useState(false);
  const [formData, _formData] = useState({});
  const [loadingSave, _loadingSave] = useState(false);

  const queryList = () => {
    if (!model.Id) {
      return;
    }
    _loading(true);
    http.apiRequest
      .post('/mall-admin/spec/list', {
        Id: model.Id,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _data(res.data.Data || []);
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const deleteSpec = (record: any) => {
    if (!confirm('确定删除规格')) {
      return;
    }
    _loading(true);
    http.apiRequest
      .post('/mall-admin/spec/delete', {
        Id: record.Id,
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
        _loading(false);
      });
  };

  useEffect(() => {
    queryList();
  }, [model]);

  const [form] = Form.useForm();

  const save = (row: any) => {
    _loadingSave(true);

    row.GoodsId = model.Id;
    http.apiRequest
      .post('/mall-admin/spec/save', {
        ...row,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('保存成功');
          _showForm(false);
          queryList();
        }
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  useEffect(() => {
    form.resetFields();
    if (formData) {
      form.setFieldsValue(formData);
    }
  }, [formData]);

  const columns: ColumnType<any>[] = [
    {
      title: '名称',
      render: (x) => x.Name || '--',
    },
    {
      title: '描述',
      render: (x) => x.Description || '--',
    },
    {
      title: '明细',
      render: (record: any) => {
        return (
          <ValueList
            model={record}
            ok={() => {
              queryList();
            }}
          />
        );
      },
    },
    {
      title: '操作',
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
            <Button
              type="primary"
              danger
              onClick={() => {
                deleteSpec(record);
              }}
            >
              删除
            </Button>
          </Button.Group>
        );
      },
    },
  ];

  return (
    <>
      <Modal
        visible={showForm}
        onCancel={() => _showForm(false)}
        onOk={() => form.submit()}
      >
        <Spin spinning={loadingSave}>
          <Form
            form={form}
            onFinish={(e) => save(e)}
            labelCol={{ flex: '110px' }}
            labelAlign="right"
            wrapperCol={{ flex: 1 }}
          >
            <Form.Item name="Id">
              <Input type="hidden" />
            </Form.Item>
            <Form.Item
              label="名称"
              name="Name"
              rules={[
                {
                  required: true,
                },
                {
                  max: 20,
                },
              ]}
            >
              <Input />
            </Form.Item>
            <Form.Item
              label="描述"
              name="Description"
              rules={[
                {
                  max: 100,
                },
              ]}
            >
              <Input.TextArea />
            </Form.Item>
          </Form>
        </Spin>
      </Modal>
      <Card
        size="small"
        title="商品规格/属性"
        style={{
          marginBottom: 10,
        }}
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
          dataSource={data}
          pagination={false}
        />
      </Card>
    </>
  );
};
