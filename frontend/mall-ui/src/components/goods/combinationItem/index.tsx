import { GoodsCombinationDto } from '@/utils/models';
import { Typography } from '@mui/material';
import Box from '@mui/material/Box';
import XCombinationPriceRow from '../combinationPriceRow';
import XGoodsLabels from '../goodsLabels';
import XPicture from '../picture';

export default function Demo(props: { model: GoodsCombinationDto }) {
  const { model } = props;
  const goods = model.Goods || {};

  return (
    <Box
      sx={{
        bgcolor: 'background.paper',
        overflow: 'hidden',
        borderRadius: '12px',
        boxShadow: 1,
        cursor: 'pointer',
        display: 'flex',
        flexDirection: 'row',
        //alignItems: 'center',
        justifyContent: 'flex-start',
        '&:hover': {
          border: (theme) => `1px solid ${theme.palette.primary.main}`,
        },
      }}
    >
      <Box
        sx={{
          width: {
            xs: '120px',
            sm: '170px',
            md: '230px',
          },
        }}
      >
        <XPicture model={goods} />
      </Box>
      <Box
        sx={{
          p: 1,
          width: '100%',
        }}
      >
        <Typography
          variant="subtitle2"
          component={'div'}
          gutterBottom
          sx={{
            fontWeight: 'lighter',
          }}
        >
          {goods?.Name || '--'}
        </Typography>
        <XCombinationPriceRow model={model} />
        <XGoodsLabels model={goods} />
      </Box>
    </Box>
  );
}
