import u from '@/utils';
import { Descriptions } from 'antd';
import XOrderStatusTag from '@/components/status/order';
import XOrderPaymentTag from '@/components/status/order/payment';
import XOrderDeliveryTag from '@/components/status/order/delivery';

const App = (props: any) => {
  const { model } = props;

  return (
    <>
      <Descriptions bordered size="small" style={{ marginBottom: 10 }}>
        <Descriptions.Item label="订单号">
          {model.OrderSn || '--'}
        </Descriptions.Item>
        <Descriptions.Item label="下单时间">
          {u.dateTimeFromNow(model.CreationTime)}
        </Descriptions.Item>
        <Descriptions.Item label="最后变更时间">
          {u.dateTimeFromNow(model.LastModificationTime)}
        </Descriptions.Item>
        <Descriptions.Item label="订单状态">
          <XOrderStatusTag model={model} />
        </Descriptions.Item>
        <Descriptions.Item label="支付状态">
          <XOrderPaymentTag model={model} />
        </Descriptions.Item>
        <Descriptions.Item label="配送状态">
          <XOrderDeliveryTag model={model} />
        </Descriptions.Item>
        <Descriptions.Item label="备注">
          {model.Remark || '--'}
        </Descriptions.Item>
        <Descriptions.Item label="买家姓名">
          {model.ShippingAddressContactName || '--'}
        </Descriptions.Item>
        <Descriptions.Item label="买家电话">
          {model.ShippingAddressContact || '--'}
        </Descriptions.Item>
        <Descriptions.Item label="配送地址">
          {model.ShippingAddressDetail || '--'}
        </Descriptions.Item>
      </Descriptions>
    </>
  );
};

export default App;
