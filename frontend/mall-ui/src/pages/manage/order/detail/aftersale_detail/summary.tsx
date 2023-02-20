import u from '@/utils';
import { Descriptions } from 'antd';
import XAfterSalesStatus from '@/components/status/order/aftersale';

const App = (props: any) => {
  const { model } = props;

  return (
    <>
      <Descriptions bordered size="small">
        <Descriptions.Item label="状态">
          <XAfterSalesStatus model={model} />
        </Descriptions.Item>
        <Descriptions.Item label="售后诉求">
          {model.RequestedAction || '--'}
        </Descriptions.Item>
        <Descriptions.Item label="退货理由">
          {model.ReasonForReturn || '--'}
        </Descriptions.Item>
        <Descriptions.Item label="备注">
          {model.UserComments || '--'}
        </Descriptions.Item>
        <Descriptions.Item label="创建时间">
          {u.dateTimeFromNow(model.CreationTime)}
        </Descriptions.Item>
        <Descriptions.Item label="最后变更时间">
          {u.dateTimeFromNow(model.LastModificationTime)}
        </Descriptions.Item>
      </Descriptions>
    </>
  );
};

export default App;
