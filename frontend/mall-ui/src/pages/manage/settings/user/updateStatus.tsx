import http from '@/utils/http';
import { EditOutlined } from '@ant-design/icons';
import { Button, Form, Input, message, Modal, Space, Switch, Tag } from 'antd';
import { useEffect, useState } from 'react';

const AdminPage = (props: any) => {
  const { ok, model } = props;
  const [loading, _loading] = useState(false);
  const [show, _show] = useState(false);

  const [form] = Form.useForm();

  const save = (e: any) => {
    _loading(true);
    http.adminRequest
      .post('/user/update-status', e)
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('保存成功');
          ok && ok();
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    form.resetFields();
    if (model) {
      form.setFieldsValue({ ...model });
    }
  }, [model]);

  return (
    <>
      <Modal
        title="修改状态"
        confirmLoading={loading}
        open={show}
        onCancel={() => {
          _show(false);
        }}
        onOk={() => {
          form.submit();
        }}
        footer={false}
        style={{
          padding: 0,
        }}
      >
        <Form
          form={form}
          onFinish={(e) => {
            save(e);
          }}
        >
          <Form.Item hidden name={'Id'}>
            <Input />
          </Form.Item>
          <Form.Item label="是否激活" valuePropName="checked" name={'IsActive'}>
            <Switch />
          </Form.Item>
          <Form.Item
            label="是否删除"
            valuePropName="checked"
            name={'IsDeleted'}
          >
            <Switch />
          </Form.Item>
        </Form>
      </Modal>
      <Space>
        {model.IsActive ? (
          <Tag color="green">正常</Tag>
        ) : (
          <Tag color="error">禁用</Tag>
        )}
        {model.IsDeleted ? (
          <Tag color="error">被删除</Tag>
        ) : (
          <Tag color="green">正常</Tag>
        )}
        <Button
          icon={<EditOutlined />}
          onClick={() => {
            _show(true);
          }}
        ></Button>
      </Space>
    </>
  );
};

export default AdminPage;
