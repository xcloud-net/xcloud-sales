import http from '@/utils/http';
import { Form, Input, message, Modal, Button } from 'antd';
import { useEffect, useState } from 'react';
import { SaveOutlined } from '@ant-design/icons';

const AdminPage = (props: any) => {
  const { show, hide, ok } = props;
  const [loading, _loading] = useState(false);
  const [form] = Form.useForm();

  const save = (e: any) => {
    _loading(true);
    http.adminRequest
      .post('/user/create', e)
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
    show && form.resetFields();
  }, [show]);

  return (
    <>
      <Modal
        title="新增用户"
        confirmLoading={loading}
        open={show}
        onCancel={() => {
          hide && hide();
        }}
        footer={
          <>
            <Button
              type="primary"
              onClick={() => {
                form.submit();
              }}
              icon={<SaveOutlined />}
            >
              保存
            </Button>
          </>
        }
      >
        <Form
          form={form}
          onFinish={(e) => {
            save(e);
          }}
        >
          <Form.Item
            label="用户名"
            name="IdentityName"
            rules={[
              {
                required: true,
                message: '请输入用户名',
              },
              {
                pattern: new RegExp('^[a-zA-Z0-9_]{3,20}$'),
                message: '用户名必须是3-16位字母、数字或下划线',
              },
            ]}
          >
            <Input />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default AdminPage;
