import { GoodsDto } from '@/utils/models';
import { Typography } from '@mui/material';
import Box from '@mui/material/Box';
import XCombination from '../combinations';
import XGoodsLabels from '../goodsLabels';
import XPicture from '../picture';

export default function Demo(props: { model: GoodsDto; count?: number }) {
  const { model, count } = props;

  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'flex-start',
        justifyContent: 'flex-start',
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
        <XPicture model={model} />
      </Box>
      <Box
        sx={{
          px: 1,
          width: '100%',
        }}
      >
        <Typography variant="subtitle2" component={'div'} gutterBottom sx={{}}>
          {model.Name || '--'}
        </Typography>
        <XCombination model={model} count={count} />
        <XGoodsLabels model={model} />
      </Box>
    </Box>
  );
}
