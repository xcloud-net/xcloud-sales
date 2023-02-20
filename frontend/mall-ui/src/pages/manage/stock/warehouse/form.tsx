import http from '@/utils/http';
import { WarehouseDto } from '@/utils/models';
import { Form, Input, message, Modal } from 'antd';
import { useEffect, useState } from 'react';

export default (props: {
  show: boolean;
  hide: any;
  data: WarehouseDto;
  ok: any;
}) => {
  const { show, hide, data, ok } = props;
  const [loading, _loading] = useState(false);

  const [form] = Form.useForm();

  const save = (row: WarehouseDto) => {
    _loading(true);

    console.log(row);
    http.apiRequest
      .post('/mall-admin/warehouse/save', {
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
      <Modal
        open={show}
        confirmLoading={loading}
        onCancel={() => hide()}
        onOk={() => form.submit()}
      >
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
            rules={[{ required: true }, { max: 10 }]}
          >
            <Input />
          </Form.Item>
          <Form.Item label="地址" name="Address" rules={[{ max: 500 }]}>
            <Input />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};
