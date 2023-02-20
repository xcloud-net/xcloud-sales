import { SearchOutlined } from '@ant-design/icons';
import { Button, Card, Form, Input } from 'antd';
import { useEffect } from 'react';

const App = (props: any) => {
  const { query, onSearch } = props;

  const [form] = Form.useForm();

  const triggerSearch = (e: any) => {
    e.Page = 1;
    onSearch && onSearch(e);
  };

  useEffect(() => {
    form.setFieldsValue(query);
  }, []);

  return (
    <Card bordered={false} style={{ marginBottom: 10 }}>
      <Form
        form={form}
        name="basic"
        labelCol={{
          span: 8,
        }}
        wrapperCol={{
          span: 16,
        }}
        initialValues={{
          remember: true,
        }}
        onFinish={(e) => triggerSearch(e)}
        autoComplete="off"
        layout="inline"
      >
        <Form.Item label="关键词" name="Name">
          <Input />
        </Form.Item>

        <Form.Item
          wrapperCol={{
            offset: 8,
            span: 16,
          }}
        >
          <Button type="primary" htmlType="submit" icon={<SearchOutlined />}>
            搜索
          </Button>
        </Form.Item>
      </Form>
    </Card>
  );
};

export default App;
