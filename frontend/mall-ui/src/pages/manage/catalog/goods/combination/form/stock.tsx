import u from '@/utils';
import http from '@/utils/http';
import { EditOutlined } from '@ant-design/icons';
import { Button, Form, InputNumber, Modal, Space, Tag, message } from 'antd';
import { useEffect, useState } from 'react';

export default (props: any) => {
  const {model, ok} = props;
  const [show, _show] = useState(false);
  const [loading, _loading] = useState(false);

  const [form] = Form.useForm();

  const saveStatus = (e: any) => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/combination/update-stock', {
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
        {model.StockQuantity > 0 && <Tag color="green">{model.StockQuantity || 0}</Tag>}
        {model.StockQuantity <= 0 && <Tag color="red">无库存</Tag>}
      </Space>
      <Button
        onClick={() => {
          _show(true);
        }}
        icon={<EditOutlined/>}
        size="small"
      ></Button>
      <Modal
        title='设置库存'
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
          <Form.Item label="库存" name="StockQuantity">
            <InputNumber/>
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};
