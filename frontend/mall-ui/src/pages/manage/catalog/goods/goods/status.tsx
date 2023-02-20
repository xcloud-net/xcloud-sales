import u from '@/utils';
import http from '@/utils/http';
import { Button, Form, message, Modal, Spin, Switch, Space, Tag } from 'antd';
import { useEffect, useState } from 'react';
import { EditOutlined } from '@ant-design/icons';

export default (props: any) => {
  const { model, ok } = props;
  const [show, _show] = useState(false);
  const [loading, _loading] = useState(false);

  const [form] = Form.useForm();

  const saveStatus = (e: any) => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/goods/update-status', {
        GoodsId: model.Id,
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
        {model.Published && <Tag color="green">在售</Tag>}
        {model.IsDeleted && <Tag color="red">被删除</Tag>}
        {model.IsHot && <Tag color="red">热销</Tag>}
        {model.IsNew && <Tag color="red">最近上新</Tag>}
        {model.StickyTop && <Tag color="red">置顶</Tag>}
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
              label="是否可用"
              valuePropName="checked"
              name="Published"
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
            <Form.Item label="上新" valuePropName="checked" name="IsNew">
              <Switch />
            </Form.Item>
            <Form.Item label="热销" valuePropName="checked" name="IsHot">
              <Switch />
            </Form.Item>
            <Form.Item label="置顶" valuePropName="checked" name="StickyTop">
              <Switch />
            </Form.Item>
          </Form>
        </Spin>
      </Modal>
    </>
  );
};
