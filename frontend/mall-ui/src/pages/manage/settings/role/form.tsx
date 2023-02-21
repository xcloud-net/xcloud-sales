import http from '@/utils/http';
import { Form, Input, message, Modal, Spin } from 'antd';
import React, { useEffect, useState } from 'react';
import { RoleDto } from '@/utils/models';

export default (props: any) => {
  const { show, hide, data, ok } = props;
  const [loading, _loading] = useState(false);

  const [form] = Form.useForm<RoleDto>();

  const save = (row: RoleDto) => {
    _loading(true);

    console.log(row);
    http.apiRequest
      .post('/sys/role/save', {
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
      <Modal open={show} onCancel={() => hide()} onOk={() => form.submit()}>
        <Spin spinning={loading}>
          <Form
            form={form}
            onFinish={(e) => save(e)}
            labelCol={{ flex: '110px' }}
            labelAlign='right'
            wrapperCol={{ flex: 1 }}
          >
            <Form.Item name='Id'>
              <Input type='hidden' />
            </Form.Item>
            <Form.Item
              label='名称'
              name='Name'
              rules={[{ required: true }, { max: 10 }]}
            >
              <Input />
            </Form.Item>
            <Form.Item label='描述' name='Description' rules={[{ max: 50 }]}>
              <Input />
            </Form.Item>
          </Form>
        </Spin>
      </Modal>
    </>
  );
};
