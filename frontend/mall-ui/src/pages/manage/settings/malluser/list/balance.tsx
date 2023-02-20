import {
  message,
  Modal,
  Tag,
  Radio,
  InputNumber,
  Input,
  Row,
  Col,
  Alert,
} from 'antd';
import { useEffect, useState } from 'react';

import http from '@/utils/http';

export default (props: any) => {
  const { model, ok } = props;

  const [loading, _loading] = useState(false);
  const [show, _show] = useState(false);

  const [formData, _formData] = useState({
    ActionType: 1,
    Balance: 0,
    UserId: 0,
    Message: '',
  });

  const save = () => {
    if (formData.Balance <= 0) {
      message.error('金额不能为0');
      return;
    }

    _loading(true);
    formData.UserId = model.Id;
    http.apiRequest
      .post('/mall-admin/balance/charge-or-deduct', {
        ...formData,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('保存成功');
          _show(false);
          ok && ok();
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    show && _formData({ ActionType: 1, Balance: 0, UserId: 0, Message: '' });
  }, [show]);

  const balanceMaxInput = formData.ActionType == -1 ? model.Balance : 9999999;

  return (
    <>
      <Modal
        title="调整"
        open={show}
        onCancel={() => {
          _show(false);
        }}
        confirmLoading={loading}
        onOk={() => {
          save();
        }}
      >
        <Alert
          message={`当前余额：${model.Balance}`}
          style={{ marginBottom: 10 }}
        ></Alert>
        <Row gutter={[15, 15]}>
          <Col span={24}>
            <InputNumber
              title="金额"
              min={0}
              max={balanceMaxInput}
              value={formData.Balance}
              onChange={(e) => {
                _formData({
                  ...formData,
                  Balance: e || 0,
                });
              }}
            />
          </Col>
          <Col span={24}>
            <Radio.Group
              value={formData.ActionType}
              onChange={(e) => {
                _formData({
                  ...formData,
                  ActionType: e.target.value,
                });
              }}
            >
              <Radio value={-1}>扣减</Radio>
              <Radio value={1}>充值</Radio>
            </Radio.Group>
          </Col>
          <Col span={24}>
            <Input.TextArea
              placeholder="描述一下这次调整..."
              value={formData.Message}
              onChange={(e) => {
                _formData({
                  ...formData,
                  Message: e.target.value,
                });
              }}
            />
          </Col>
        </Row>
      </Modal>
      <Tag
        color={'success'}
        onClick={() => {
          _show(true);
        }}
        style={{
          cursor: 'pointer',
        }}
      >
        {model.Balance}
      </Tag>
    </>
  );
};
