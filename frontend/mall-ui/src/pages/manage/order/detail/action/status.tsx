import u from '@/utils';
import utils from '@/utils/order';
import { Alert, Form, Input, Modal, Select } from 'antd';

import * as React from 'react';

export default function Animations(props: any) {
  const { model, ok, show, hide } = props;

  const [loadingSave, _loadingSave] = React.useState(false);

  const [comment, _comment] = React.useState('');
  const [status, _status] = React.useState(-1);
  const [paymentstatus, _paymentstatus] = React.useState(-1);
  const [shippedstatus, _shippedstatus] = React.useState(-1);

  const { OrderStatusId, ShippingStatusId, PaymentStatusId } = model;

  React.useEffect(() => {
    _status(OrderStatusId);
    _shippedstatus(ShippingStatusId);
    _paymentstatus(PaymentStatusId);
  }, [model]);

  const save = () => {
    _loadingSave(true);
    u.http.apiRequest
      .post('/mall-admin/order/dangerously-update-status', {
        Id: model.Id,
        OrderStatus: status,
        PaymentStatus: paymentstatus,
        DeliveryStatus: shippedstatus,
        Comment: comment,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          hide && hide();
          ok && ok();
        });
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  return (
    <>
      <Modal
        open={show}
        title="⚠️强制修改状态"
        onCancel={() => {
          hide && hide();
        }}
        onOk={() => {
          save();
        }}
        okText="危险：强制修改状态"
        confirmLoading={loadingSave}
      >
        <Alert
          message="错误的修改状态可能造成数据紊乱，请谨慎操作！"
          type="warning"
          style={{ marginBottom: 10 }}
        />

        <Form>
          <Form.Item>
            <Input.TextArea
              autoFocus
              placeholder="修改理由"
              value={comment}
              onChange={(e) => _comment(e.target.value)}
            />
          </Form.Item>
          <Form.Item>
            <Select
              value={status}
              onChange={(e) => {
                _status(e);
              }}
              placeholder="订单状态"
            >
              {u.map(utils.allStatus.orderStatus, (x, index) => (
                <Select.Option key={index} value={x.status}>
                  {x.name}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item>
            <Select
              value={paymentstatus}
              onChange={(e) => {
                _paymentstatus(e);
              }}
              placeholder="支付状态"
            >
              {u.map(utils.allStatus.paymentStatus, (x, index) => (
                <Select.Option key={index} value={x.status}>
                  {x.name}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item>
            <Select
              value={shippedstatus}
              onChange={(e) => {
                _shippedstatus(e);
              }}
              placeholder="配送状态"
            >
              {u.map(utils.allStatus.ShippingStatus, (x, index) => (
                <Select.Option key={index} value={x.status}>
                  {x.name}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
}
