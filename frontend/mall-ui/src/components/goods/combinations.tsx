import XCombinationPriceRow from '@/components/goods/combinationPriceRow';
import u from '@/utils';
import { GoodsDto } from '@/utils/models';
import { Box, Typography } from '@mui/material';

const index = function (props: { model: GoodsDto; count?: number }) {
  const { model, count } = props;

  if (!model || !model.Id) {
    return null;
  }

  const takeCount = count || 1;

  const combinations = model.GoodsSpecCombinations || [];
  const combinationsCount: number = combinations.length;

  return (
    <>
      <Box sx={{}}>
        {combinationsCount <= 0 && (
          <Typography variant="caption" color="text.secondary">
            暂无可售
          </Typography>
        )}
        {combinationsCount > 0 &&
          u.map(u.take(combinations, takeCount), (x, index) => (
            <Box sx={{}} key={index}>
              <XCombinationPriceRow model={x} />
            </Box>
          ))}
        {combinationsCount > takeCount && (
          <Typography
            variant="caption"
            color="primary"
          >{`一共${combinationsCount}种款式`}</Typography>
        )}
      </Box>
    </>
  );
};

export default index;
