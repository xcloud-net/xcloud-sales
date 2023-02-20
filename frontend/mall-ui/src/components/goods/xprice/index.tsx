import { GoodsCombinationDto } from '@/utils/models';
import { Box, Tooltip, Typography } from '@mui/material';
import currency from 'currency.js';
//import XBox from './box';
//import XFormated from './formated';

const index = function (props: { model: GoodsCombinationDto }) {
  const { model } = props;

  const { Price, GradeName, GradePrice } = model;
  const originPrice = Price || 0;
  const finalPrice = GradePrice || originPrice;
  const offset = finalPrice - originPrice;

  const wrapTooltipOrNot = (com: any) => {
    if (offset == 0) {
      return com;
    }
    return <Tooltip title={`针对${GradeName || '--'}的价格`}>{com}</Tooltip>;
  };

  if (model.PriceIsHidden) {
    return (
      <Typography
        variant="overline"
        color={'text.disabled'}
        sx={{
          whiteSpace: 'nowrap',
          fontWeight: 'lighter',
        }}
      >
        价格会员可见
      </Typography>
    );
  }

  return (
    <>
      <Box
        sx={{
          display: 'inline-block',
        }}
      >
        {offset != 0 && (
          <Typography
            variant="caption"
            sx={{
              color: 'text.disabled',
              textDecoration: 'line-through',
              fontSize: '9px',
              whiteSpace: 'nowrap',
              fontWeight: 'lighter',
            }}
          >
            {`原价${currency(originPrice, {
              separator: ',',
              symbol: '￥',
              precision: 2,
            }).format()}`}
          </Typography>
        )}
        {wrapTooltipOrNot(
          <Typography
            color="primary"
            variant="button"
            sx={{
              whiteSpace: 'nowrap',
              fontWeight: 'lighter',
            }}
          >
            {`${currency(finalPrice, {
              separator: ',',
              symbol: '￥',
              precision: 2,
            }).format()}`}
          </Typography>,
        )}
      </Box>
    </>
  );
};

export default index;
