import u from '@/utils';
import http from '@/utils/http';
import { Button, Form, message, Modal, Spin, Switch, Space, Tag } from 'antd';
import { useEffect, useState } from 'react';
import { EditOutlined } from '@ant-design/icons';
import { OrderDto } from '@/utils/models';

export default (props: { model: OrderDto; ok: any }) => {
  const { model, ok } = props;
  const [show, _show] = useState(false);
  const [loading, _loading] = useState(false);

  const [form] = Form.useForm();

  const saveStatus = (e: any) => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/aftersale/update-status', {
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
        {model.HideForAdmin && <Tag color="red">管理员隐藏</Tag>}
        {model.IsDeleted && <Tag color="red">用户删除</Tag>}
      </Space>
      <Button
        onClick={() => {
          _show(true);
        }}
        icon={<EditOutlined />}
        size="small"
      ></Button>
      <Modal
        confirmLoading={loading}
        open={show}
        onCancel={() => _show(false)}
        onOk={() => form.submit()}
      >
        <Spin spinning={loading}>
          <Form
            form={form}
            onFinish={(e) => saveStatus(e)}
            labelCol={{ flex: '110px' }}
            labelAlign="right"
            wrapperCol={{ flex: 1 }}
          >
            <Form.Item
              label="对管理员隐藏"
              valuePropName="checked"
              name="HideForAdmin"
            >
              <Switch />
            </Form.Item>
            <Form.Item
              label="对用户隐藏"
              valuePropName="checked"
              name="IsDeleted"
            >
              <Switch />
            </Form.Item>
          </Form>
        </Spin>
      </Modal>
    </>
  );
};
