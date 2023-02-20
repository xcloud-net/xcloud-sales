import u from '@/utils';
import { CouponDto, PromotionDto } from '@/utils/models';
import { Box } from '@mui/material';
import { Swiper } from 'antd-mobile';
import XLoading from '../components/loading';
import XCouponItem from './couponItem';
import XPromotionItem from './promotionItem';

export default ({
  coupon,
  promotion,
  loading,
  ok,
}: {
  loading?: boolean;
  coupon: CouponDto[];
  promotion: PromotionDto[];
  ok?: any;
}) => {
  const renderItems = () => {
    var items: any[] = [];
    items = [
      ...items,
      ...(promotion || []).map((x, index) => (
        <Box sx={{ p: 2 }}>
          <XPromotionItem model={x} ok={() => {}} />
        </Box>
      )),
    ];

    items = [
      ...items,
      ...(coupon || []).map((x, index) => (
        <Box sx={{ p: 2 }}>
          <XCouponItem
            model={x}
            ok={() => {
              ok && ok();
            }}
          />
        </Box>
      )),
    ];

    return items.map((x, index) => (
      <Swiper.Item key={`item-${index}`}>{x}</Swiper.Item>
    ));
  };

  if (loading) {
    return <XLoading />;
  }

  if (u.isEmpty(promotion) && u.isEmpty(coupon)) {
    return null;
  }

  return (
    <>
      <Box sx={{ mb: 1, px: 0 }}>
        <Swiper
          trackOffset={10}
          slideSize={90}
          defaultIndex={0}
          indicator={() => null}
        >
          {renderItems()}
        </Swiper>
      </Box>
    </>
  );
};
