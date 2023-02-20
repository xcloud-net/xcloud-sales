import u from '@/utils';
import http from '@/utils/http';
import { MallSettingsDto } from '@/utils/models';
import { Button, Card, Form, Input, message, Switch } from 'antd';
import { useEffect, useState } from 'react';
import { SaveOutlined } from '@ant-design/icons';

export default (props: any) => {
  const [data, _data] = useState<MallSettingsDto>({});
  const [loading, _loading] = useState(false);
  const [loadingSave, _loadingSave] = useState(false);

  const [form] = Form.useForm();

  useEffect(() => {
    data && form.setFieldsValue(data);
  }, [data]);

  const queryData = () => {
    _loading(true);

    http.apiRequest
      .post('/mall-admin/setting/mall-settings', {})
      .then((res) => {
        _data(res.data.Data || {});
      })
      .finally(() => {
        _loading(false);
      });
  };

  const saveSettings = (e: MallSettingsDto) => {
    _loadingSave(true);

    http.apiRequest
      .post('/mall-admin/setting/save-mall-settings', {
        ...e,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          message.success('保存成功');
          queryData();
        });
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  useEffect(() => {
    queryData();
  }, []);

  return (
    <>
      <Card
        title="商城设置"
        loading={loading}
        style={{
          marginBottom: 10,
        }}
        extra={
          <Button
            icon={<SaveOutlined />}
            loading={loadingSave}
            type="primary"
            onClick={() => {
              form.submit();
            }}
          >
            保存设置
          </Button>
        }
      >
        <Form
          form={form}
          onFinish={(e) => {
            saveSettings(e);
          }}
          labelCol={{ flex: '110px' }}
          labelAlign="right"
          wrapperCol={{ flex: 1 }}
        >
          <Form.Item label="首页提示" name="HomePageNotice">
            <Input />
          </Form.Item>
          <Form.Item
            label="首页分类"
            tooltip="输入分类的SeoNames，多个用英文逗号分隔"
            name="HomePageCategorySeoNames"
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="首页轮播图"
            name="HomeSliderImages"
            tooltip="图片地址，以逗号分隔"
          >
            <Input.TextArea />
          </Form.Item>

          <Form.Item label="下单页提示" name="PlaceOrderNotice">
            <Input />
          </Form.Item>
          <Form.Item label="登录页提示" name="LoginNotice">
            <Input />
          </Form.Item>
          <Form.Item label="注册页提示" name="RegisterNotice">
            <Input />
          </Form.Item>
          <Form.Item label="商品详情页提示" name="GoodsDetailNotice">
            <Input />
          </Form.Item>
          <Form.Item
            label="价格不登陆可见"
            name="DisplayPriceForGuest"
            valuePropName="checked"
          >
            <Switch />
          </Form.Item>
          <Form.Item
            label="禁止下单"
            name="PlaceOrderDisabled"
            valuePropName="checked"
          >
            <Switch />
          </Form.Item>
          <Form.Item
            label="禁止售后"
            name="AftersaleDisabled"
            valuePropName="checked"
          >
            <Switch />
          </Form.Item>
        </Form>
      </Card>
    </>
  );
};
