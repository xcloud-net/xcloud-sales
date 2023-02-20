import u from '@/utils';
import http from '@/utils/http';
import { EditOutlined } from '@ant-design/icons';
import { Button, Form, Modal, Space, Switch, Tag, message } from 'antd';
import { useEffect, useState } from 'react';

export default (props: any) => {
  const {model, ok} = props;
  const [show, _show] = useState(false);
  const [loading, _loading] = useState(false);

  const [form] = Form.useForm();

  const saveStatus = (e: any) => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/combination/update-status', {
        Id: model.Id,
        ...e,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          message.success('修改成功');
          _show(false);
          ok && ok();
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    form.resetFields();
    if (model) {
      form.setFieldsValue(model);
    }
  }, [model]);

  return (
    <>
      <Space direction="vertical">
        {model.IsActive && <Tag color="green">可用</Tag>}
        {model.IsDeleted && <Tag color="red">被删除</Tag>}
      </Space>
      <Button
        onClick={() => {
          _show(true);
        }}
        icon={<EditOutlined/>}
        size="small"
      ></Button>
      <Modal
        title='修改状态'
        confirmLoading={loading}
        open={show}
        onCancel={() => _show(false)}
        onOk={() => form.submit()}
      >
        <Form
          form={form}
          onFinish={(e) => saveStatus(e)}
          labelCol={{flex: '110px'}}
          labelAlign="right"
          wrapperCol={{flex: 1}}
        >
          <Form.Item label="是否可用" valuePropName="checked" name="IsActive">
            <Switch/>
          </Form.Item>
          <Form.Item
            label="对用户隐藏"
            valuePropName="checked"
            name="IsDeleted"
          >
            <Switch/>
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};
