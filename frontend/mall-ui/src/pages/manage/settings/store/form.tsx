import http from '@/utils/http';
import { Form, Input, message, Modal, Spin, Switch } from 'antd';
import React, { useEffect, useState } from 'react';

export default (props: any) => {
  const { show, hide, data, ok } = props;
  const [loading, _loading] = useState(false);

  const [form] = Form.useForm();

  const save = (row: any) => {
    _loading(true);

    console.log(row);
    http.apiRequest
      .post('/mall-admin/store/save', {
        ...row,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('保存成功');
          ok();
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    form.resetFields();
    if (data) {
      form.setFieldsValue(data);
    }
  }, [data]);

  return (
    <>
      <Modal visible={show} onCancel={() => hide()} onOk={() => form.submit()}>
        <Spin spinning={loading}>
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
            <Form.Item label="名称" name="StoreName">
              <Input />
            </Form.Item>
            <Form.Item label="网址" name="StoreUrl">
              <Input />
            </Form.Item>
            <Form.Item label="营业中" name="StoreClosed">
              <Switch />
            </Form.Item>
            <Form.Item label="是否激活" name="IsActive">
              <Switch />
            </Form.Item>
          </Form>
        </Spin>
      </Modal>
    </>
  );
};
