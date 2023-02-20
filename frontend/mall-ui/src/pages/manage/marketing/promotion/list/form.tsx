import http from '@/utils/http';
import {
  DatePicker,
  Form,
  Input,
  InputNumber,
  message,
  Modal,
  Spin,
  Switch,
} from 'antd';
import { useEffect, useState } from 'react';
import { PromotionDto } from '@/utils/models';
import u from '@/utils';

export default (props: any) => {
  const { show, hide, data, ok } = props;
  const [loading, _loading] = useState(false);

  const [form] = Form.useForm();

  const save = (row: PromotionDto) => {
    _loading(true);

    if (row.StartTime) {
      row.StartTime = u.formatDateTime(row.StartTime) || undefined;
    }

    if (row.EndTime) {
      row.EndTime = u.formatDateTime(row.EndTime) || undefined;
    }

    http.apiRequest
      .post('/mall-admin/promotion/save', {
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
      var dataCopy = { ...data };

      if (!u.isEmpty(dataCopy.StartTime)) {
        dataCopy.StartTime = u
          .dayjs(dataCopy.StartTime)
          .add(u.timezoneOffset, 'hours');
      }
      if (!u.isEmpty(dataCopy.EndTime)) {
        dataCopy.EndTime = u
          .dayjs(dataCopy.EndTime)
          .add(u.timezoneOffset, 'hours');
      }
      form.setFieldsValue(dataCopy);
    }
  }, [data]);

  return (
    <>
      <Modal open={show} onCancel={() => hide()} onOk={() => form.submit()}>
        <Spin spinning={loading}>
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
              rules={[{ required: true }, { max: 30 }]}
              tooltip="例如满100减10"
            >
              <Input />
            </Form.Item>
            <Form.Item label="描述" name="Description">
              <Input.TextArea />
            </Form.Item>
            <Form.Item
              label="排他"
              name="IsExclusive"
              valuePropName="checked"
              tooltip="不能共享其他优惠"
            >
              <Switch defaultChecked={true} />
            </Form.Item>
            <Form.Item label="开始时间" name="StartTime">
              <DatePicker allowClear />
            </Form.Item>
            <Form.Item label="结束时间" name="EndTime">
              <DatePicker allowClear />
            </Form.Item>
            <Form.Item label="排序" name="Order">
              <InputNumber defaultValue={0} />
            </Form.Item>
          </Form>
        </Spin>
      </Modal>
    </>
  );
};
