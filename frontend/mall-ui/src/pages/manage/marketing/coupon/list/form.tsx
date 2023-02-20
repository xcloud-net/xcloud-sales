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
import { CouponDto } from '@/utils/models';
import u from '@/utils';

export default (props: any) => {
  const { show, hide, data, ok } = props;
  const [loading, _loading] = useState(false);

  const [form] = Form.useForm();

  const save = (row: CouponDto) => {
    _loading(true);

    if (row.StartTime) {
      row.StartTime = u.formatDateTime(row.StartTime) || undefined;
    }

    if (row.EndTime) {
      row.EndTime = u.formatDateTime(row.EndTime) || undefined;
    }

    http.apiRequest
      .post('/mall-admin/coupon/save', {
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
              name="Title"
              rules={[{ required: true }, { max: 30 }]}
              tooltip="例如满100减10"
            >
              <Input />
            </Form.Item>
            <Form.Item
              label="金额"
              name="Value"
              rules={[{ required: true }]}
              tooltip="优惠券金额"
            >
              <InputNumber />
            </Form.Item>
            <Form.Item
              label="数量"
              name="Amount"
              rules={[{ required: true }]}
              tooltip="发行数量"
            >
              <InputNumber />
            </Form.Item>
            <Form.Item
              label="数量限制"
              name="IsAmountLimit"
              valuePropName="checked"
              tooltip="无数量限制可以无限领取"
            >
              <Switch />
            </Form.Item>
            <Form.Item label="开始时间" name="StartTime">
              <DatePicker allowClear />
            </Form.Item>
            <Form.Item label="结束时间" name="EndTime">
              <DatePicker allowClear />
            </Form.Item>
            <Form.Item label="最低消费" name="MinimumConsumption">
              <InputNumber />
            </Form.Item>
            <Form.Item
              label="领取次数"
              name="AccountIssuedLimitCount"
              tooltip="每个账号可以领取多少次"
            >
              <InputNumber />
            </Form.Item>
            <Form.Item
              label="有效期"
              name="ExpiredDaysFromIssue"
              tooltip="领取后多少天有效"
            >
              <InputNumber />
            </Form.Item>
          </Form>
        </Spin>
      </Modal>
    </>
  );
};
