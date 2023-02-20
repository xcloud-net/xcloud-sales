import u from '@/utils';
import { Typography } from '@mui/material';
import { Form, Input, InputNumber, message, Modal } from 'antd';

import * as React from 'react';

export default function Animations(props: any) {
  const { model, ok, show, hide } = props;

  const [loadingSave, _loadingSave] = React.useState(false);

  const [data, _data] = React.useState({
    ShippingMethod: '',
    ExpressName: '',
    TrackingNumber: '',
    TotalWeight: 1,
  });

  const items = u.map(model.Items, (x) => ({
    OrderItemId: x.Id,
    Quantity: x.Quantity,
  }));

  const save = () => {
    if (u.isEmpty(data.ShippingMethod) || u.isEmpty(data.ExpressName)) {
      message.error('请完善物流信息');
      return;
    }

    if (u.isEmpty(items)) {
      message.error('没有选定商品');
      return;
    }

    _loadingSave(true);
    u.http.apiRequest
      .post('/mall-admin/shipping/ship-order', {
        OrderId: model.Id,
        Items: [...items],
        ...data,
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
        title="发货"
        open={show}
        onCancel={() => hide && hide()}
        onOk={() => {
          save();
        }}
        okText="标记为已发货"
        confirmLoading={loadingSave}
      >
        <Typography variant="h6" component={'div'} gutterBottom>
          请填写物流信息
        </Typography>
        <Form>
          <Form.Item label="配送方式">
            <Input
              placeholder="配送方式"
              value={data.ShippingMethod}
              onChange={(e) =>
                _data({ ...data, ShippingMethod: e.target.value })
              }
            />
          </Form.Item>
          <Form.Item label="物流名称">
            <Input
              placeholder="物流名称"
              value={data.ExpressName}
              onChange={(e) => _data({ ...data, ExpressName: e.target.value })}
            />
          </Form.Item>
          <Form.Item label="物流单号">
            <Input
              placeholder="物流单号"
              value={data.TrackingNumber}
              onChange={(e) =>
                _data({ ...data, TrackingNumber: e.target.value })
              }
            />
          </Form.Item>
          <Form.Item label="包裹重量（公斤）">
            <InputNumber
              placeholder="物品重量(KG)"
              value={data.TotalWeight}
              onChange={(e) => _data({ ...data, TotalWeight: e || 0 })}
            />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
}
