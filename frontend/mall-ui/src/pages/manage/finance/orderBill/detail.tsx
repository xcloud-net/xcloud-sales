import { OrderBillDto } from '@/utils/models';
import utils from '@/utils/order';
import { Descriptions } from 'antd';

export default ({ model }: { model: OrderBillDto }) => {
  const renderPayment = () => {
    if (!model.Paid) {
      return null;
    }
    return (
      <Descriptions title="支付信息" bordered style={{ marginBottom: 10 }}>
        <Descriptions.Item label="交易单号">
          {model.PaymentTransactionId || '--'}
        </Descriptions.Item>
        <Descriptions.Item label="支付方式">
          {utils.getPaymentMethod(model.PaymentMethod)?.name || '--'}
        </Descriptions.Item>
        <Descriptions.Item label="支付时间">{model.PayTime}</Descriptions.Item>
        <Descriptions.Item label="回调信息" span={2}>
          {model.NotifyData || '--'}
        </Descriptions.Item>
      </Descriptions>
    );
  };

  const renderRefund = () => {
    if (!model.Refunded) {
      return null;
    }
    return (
      <Descriptions title="退款信息" bordered style={{ marginBottom: 10 }}>
        <Descriptions.Item label="退款交易号">
          {model.RefundTransactionId || '--'}
        </Descriptions.Item>
        <Descriptions.Item label="退款时间">
          {model.RefundTime}
        </Descriptions.Item>
        <Descriptions.Item label="回调信息" span={2}>
          {model.RefundNotifyData || '--'}
        </Descriptions.Item>
      </Descriptions>
    );
  };

  return (
    <>
      {renderPayment()}
      {renderRefund()}
    </>
  );
};
