import { SearchOutlined } from '@ant-design/icons';
import { Button, Card, Col, Form, Input, InputNumber, Row, Space } from 'antd';
import { useEffect } from 'react';

const App = (props: any) => {
  const { query, onSearch } = props;

  const [form] = Form.useForm();

  const triggerSearch = (e: any) => {
    var data = { ...e };
    data.Page = 1;

    onSearch && onSearch(data);
  };

  useEffect(() => {
    form.setFieldsValue(query);
  }, []);

  return (
    <Card bordered={false} style={{ marginBottom: 10 }}>
      <Form
        form={form}
        labelCol={{
          span: 8,
        }}
        onFinish={(e) => triggerSearch(e)}
        autoComplete="off"
      >
        <Row gutter={10}>
          <Col span={6}>
            <Form.Item label="关键词" name="Keywords">
              <Input />
            </Form.Item>
          </Col>
          <Col span={6}>
            <Form.Item label="Sku" name="Sku">
              <Input />
            </Form.Item>
          </Col>

          <Col span={6}>
            <Form.Item label="最大库存" name="StockQuantityLessThanOrEqualTo">
              <InputNumber min={0} />
            </Form.Item>
          </Col>
          <Col span={6}>
            <Form.Item
              label="最小库存"
              name="StockQuantityGreaterThanOrEqualTo"
            >
              <InputNumber min={0} />
            </Form.Item>
          </Col>

          <Col span={6}>
            <Space>
              <Button
                type="primary"
                htmlType="submit"
                icon={<SearchOutlined />}
              >
                搜索
              </Button>
              <Button htmlType="reset">重置</Button>
            </Space>
          </Col>
          <Col span={6}></Col>
          <Col span={6}></Col>
        </Row>
      </Form>
    </Card>
  );
};

export default App;
