import { SearchOutlined } from '@ant-design/icons';
import {
  Button,
  Card,
  Col,
  Form,
  Input,
  Row,
  Select,
  Checkbox,
  Space,
  InputNumber,
} from 'antd';
import { useEffect } from 'react';

const App = (props: any) => {
  const { query, onSearch } = props;

  const [form] = Form.useForm();

  const triggerSearch = (e: any) => {
    var data = { ...e };
    data.Page = 1;
    //publish
    var published = null;
    if (data.IsPublished == 1) {
      published = true;
    } else if (data.IsPublished == -1) {
      published = false;
    }
    data.IsPublished = published;
    //hot
    data.IsHot = null;
    if (e.IsHot == 1) {
      data.IsHot = true;
    } else if (e.IsHot == -1) {
      data.IsHot = false;
    }
    //new
    data.IsNew = null;
    if (e.IsNew == 1) {
      data.IsNew = true;
    } else if (e.IsNew == -1) {
      data.IsNew = false;
    }

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
            <Form.Item label="上下架" name="IsPublished">
              <Select defaultValue={0}>
                <Select.Option value={0}>全部</Select.Option>
                <Select.Option value={1}>上架</Select.Option>
                <Select.Option value={-1}>下架</Select.Option>
              </Select>
            </Form.Item>
          </Col>

          <Col span={6}>
            <Form.Item label="是否热销" name="IsHot">
              <Select defaultValue={0}>
                <Select.Option value={0}>全部</Select.Option>
                <Select.Option value={1}>热销中</Select.Option>
                <Select.Option value={-1}>不是热销</Select.Option>
              </Select>
            </Form.Item>
          </Col>
          <Col span={6}>
            <Form.Item label="是否新品" name="IsNew">
              <Select defaultValue={0}>
                <Select.Option value={0}>全部</Select.Option>
                <Select.Option value={1}>新品</Select.Option>
                <Select.Option value={-1}>非新品</Select.Option>
              </Select>
            </Form.Item>
          </Col>
          <Col span={6}>
            <Form.Item
              label="没有品牌"
              name="WithoutBrand"
              valuePropName="checked"
            >
              <Checkbox />
            </Form.Item>
          </Col>

          <Col span={6}>
            <Form.Item
              label="没有分类"
              name="WithoutCategory"
              valuePropName="checked"
            >
              <Checkbox />
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
