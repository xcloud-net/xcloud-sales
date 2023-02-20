import { SearchOutlined } from '@ant-design/icons';
import { Button, Card, Form, Switch } from 'antd';
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
        initialValues={{
          remember: true,
        }}
        onFinish={(e) => triggerSearch(e)}
        autoComplete="off"
        layout="inline"
      >
        <Form.Item label="是否使用" name="Name" valuePropName="checked">
          <Switch />
        </Form.Item>

        <Form.Item>
          <Button type="primary" htmlType="submit" icon={<SearchOutlined />}>
            搜索
          </Button>
        </Form.Item>
      </Form>
    </Card>
  );
};

export default App;
