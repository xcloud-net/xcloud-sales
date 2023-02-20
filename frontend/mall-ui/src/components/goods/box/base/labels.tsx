import XGoodsLabels from '@/components/goods/goodsLabels';
import { GoodsDto } from '@/utils/models';
import Box from '@mui/material/Box';

export default function Demo(props: { model: GoodsDto }) {
  const { model } = props;

  return (
    <Box
      sx={{
        display: 'none',
      }}
    >
      <XGoodsLabels model={model} />
    </Box>
  );
}
