import { PromotionConditionDto } from '@/utils/models';

export default ({ data }: { data: PromotionConditionDto }) => {
  const { ConditionType, ConditionJson } = data;
  if (ConditionType == '*') {
    return <span>全部</span>;
  }
  if (ConditionType == 'order-price') {
    try {
      var param = JSON.parse(ConditionJson || '{}');
      return <span>{`订单金额不低于${param.LimitedOrderAmount || 0}`}</span>;
    } catch (e) {
      return <span>解析错误</span>;
    }
  }
  return (
    <>
      <span>未知规则</span>
    </>
  );
};
