import http from '@/utils/http';
import { Button, Form, Input, message } from 'antd';
import React, { useEffect, useState } from 'react';

const add_dict = async (data: {}) => {
  var res = await http.adminRequest.post('/dict/add', {
    ...data,
  });
  return res.data;
};

const dictPage = (props: any) => {
  const { onSuccessSave, formData } = props;

  const [loadingSave, _loadingSave] = useState(false);
  const [dictForm] = Form.useForm();

  useEffect(() => {
    dictForm.resetFields();
    dictForm.setFieldsValue(formData);
  }, [formData]);

  return (
    <>
      <Form
        form={dictForm}
        labelCol={{ span: 6 }}
        wrapperCol={{ span: 18 }}
        onFinish={async (e) => {
          _loadingSave(true);
          add_dict(e)
            .then((res) => {
              message.success('成功');
              onSuccessSave();
            })
            .finally(() => {
              _loadingSave(false);
            });
        }}
      >
        <Form.Item name="Id" hidden>
          <Input />
        </Form.Item>
        <Form.Item label="唯一标识" name="Key" rules={[{ required: true, message: '请输入' }]}>
          <Input />
        </Form.Item>
        <Form.Item label="名称" name="Name" rules={[{ required: true, message: '请输入' }]}>
          <Input />
        </Form.Item>
        <Form.Item label="描述" name="Description">
          <Input />
        </Form.Item>
        <Button block type="primary" htmlType="submit" loading={loadingSave}>
          保存
        </Button>
      </Form>
    </>
  );
};

export default dictPage;
