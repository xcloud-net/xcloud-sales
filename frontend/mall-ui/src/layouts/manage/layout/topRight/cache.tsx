import { ReloadOutlined } from '@ant-design/icons';
import {
  Alert,
  Button,
  Checkbox,
  Form,
  Modal,
  Space,
  Tooltip,
  message,
} from 'antd';
import { useState } from 'react';
import u from '@/utils';

var index = (props: any) => {
  const [show, _show] = useState(false);
  const [loading, _loading] = useState(false);
  const [form] = Form.useForm();

  const triggerRefresh = (e: any) => {
    _loading(true);
    u.http.apiRequest
      .post('/mall-admin/setting/refresh-view-cache', { ...e })
      .then((res) => {
        u.handleResponse(res, () => {
          message.success('缓存刷新成功！');
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  return (
    <>
      <Modal
        open={show}
        title="刷新缓存"
        confirmLoading={loading}
        okText="刷新缓存"
        onOk={() => {
          form.submit();
        }}
        onCancel={() => {
          _show(false);
        }}
      >
        <Alert
          style={{ marginBottom: 10 }}
          message="刚更新了数据，但是用户端却没有显示，可以尝试刷新缓存"
          type="warning"
        ></Alert>
        <Form
          form={form}
          onFinish={(e) => {
            triggerRefresh(e);
          }}
        >
          <Form.Item label="首页" name="Home" valuePropName="checked">
            <Checkbox>首页</Checkbox>
          </Form.Item>
          <Form.Item label="分类" name="RootCategory" valuePropName="checked">
            <Checkbox>分类</Checkbox>
          </Form.Item>
          <Form.Item label="报表统计" name="Dashboard" valuePropName="checked">
            <Checkbox>报表统计</Checkbox>
          </Form.Item>
          <Form.Item label="搜索" name="Search" valuePropName="checked">
            <Checkbox>搜索页面</Checkbox>
          </Form.Item>
        </Form>
      </Modal>
      <Space direction="horizontal">
        <Tooltip title="刷新缓存">
          <Button
            icon={<ReloadOutlined />}
            size="small"
            type="primary"
            onClick={() => {
              _show(true);
            }}
          ></Button>
        </Tooltip>
      </Space>
    </>
  );
};

export default index;
