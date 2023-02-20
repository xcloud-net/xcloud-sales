import XQR from '@/components/qrcode';
import u from '@/utils';
import { Card } from 'antd';

export default (props: any) => {
  const { model } = props;
  if (!model) {
    return null;
  }

  const renderUrl = () => {
    return u.concatUrl([
      window.location.origin,
      '/store/ucenter/prepaidcard',
      model.Id,
    ]);
  };

  return (
    <>
      <Card title="充值卡">
        <p>卡面金额</p>
        <p>
          <h3>{model.Amount}</h3>
        </p>
        <p>此卡为不记名充值卡，请勿泄露给他人！以免造成您财产损失。</p>
        <p>
          <XQR value={renderUrl()} height={128} width={128} />
        </p>
      </Card>
    </>
  );
};
