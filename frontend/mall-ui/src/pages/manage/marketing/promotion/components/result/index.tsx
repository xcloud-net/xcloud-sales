import { PromotionResultDto } from '@/utils/models';

export default ({ data }: { data: PromotionResultDto }) => {
  const { ResultType, ResultJson } = data;
  if (ResultType == 'order-discount') {
    try {
      var param = JSON.parse(ResultJson || '{}');
      return <span>{`减免${param.DiscountAmount || 0}`}元</span>;
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
