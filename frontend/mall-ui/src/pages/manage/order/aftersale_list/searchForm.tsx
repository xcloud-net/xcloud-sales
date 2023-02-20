import u from '@/utils';
import utils from '@/utils/order';
import { SearchOutlined } from '@ant-design/icons';
import { Button, Card, Col, DatePicker, Form, Row, Select, Space } from 'antd';
import { useEffect } from 'react';

const App = (props: any) => {
  const { query, onSearch } = props;

  const [form] = Form.useForm();

  const triggerSearch = (e: any) => {
    console.log('triggerSearch order', e);
    e.Page = 1;
    if (e.HideForAdmin == 1) {
      e.HideForAdmin = true;
    } else if (e.HideForAdmin == -1) {
      e.HideForAdmin = false;
    } else {
      e.HideForAdmin = null;
    }

    if (e.StartTime) {
      e.StartTime = u.formatAsUtcDateTime(e.StartTime.format('YYYY-MM-DD'));
    }
    if (e.EndTime) {
      e.EndTime = u.formatAsUtcDateTime(e.EndTime.format('YYYY-MM-DD'));
    }

    if (e.AfterSalesStatus && e.AfterSalesStatus >= 0) {
      e.Status = [e.AfterSalesStatus];
    }

    e.AfterSalesStatus = undefined;

    onSearch && onSearch(e);
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
        initialValues={{
          remember: true,
        }}
        onFinish={(e) => triggerSearch(e)}
        autoComplete="off"
      >
        <Row gutter={10}>
          <Col span={6}>
            <Form.Item label="订单状态" name="AfterSalesStatus">
              <Select defaultValue={-1}>
                <Select.Option value={-1}>全部</Select.Option>
                {u.map(utils.allStatus.AftersalesStatus, (x, index) => (
                  <Select.Option value={x.status}>{x.name}</Select.Option>
                ))}
              </Select>
            </Form.Item>
          </Col>
          <Col span={6}>
            <Form.Item label="开始时间" name="StartTime">
              <DatePicker format={'YYYY-MM-DD'} picker="date" allowClear />
            </Form.Item>
          </Col>
          <Col span={6}>
            <Form.Item label="结束时间" name="EndTime">
              <DatePicker format={'YYYY-MM-DD'} picker="date" allowClear />
            </Form.Item>
          </Col>
          <Col span={6}>
            <Form.Item label="隐藏" name="HideForAdmin">
              <Select defaultValue={0}>
                <Select.Option value={0}>全部订单</Select.Option>
                <Select.Option value={-1}>正常订单</Select.Option>
                <Select.Option value={1}>回收站订单</Select.Option>
              </Select>
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
        </Row>
      </Form>
    </Card>
  );
};

export default App;
