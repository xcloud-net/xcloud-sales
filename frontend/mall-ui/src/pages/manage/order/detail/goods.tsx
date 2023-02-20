import { Card } from 'antd';
import XGoodsList from '../components/goodsList';
import u from '@/utils';

export default (props: any) => {
  const { model } = props;

  return (
    <>
      <Card title="商品" style={{ marginBottom: 10 }} size="small">
        {u.isEmpty(model.Items) || <XGoodsList items={model.Items} />}
      </Card>
    </>
  );
};
