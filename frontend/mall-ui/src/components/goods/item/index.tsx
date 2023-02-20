import { GoodsDto } from '@/utils/models';
import Box from '@mui/material/Box';
import XBase from './base';

export default function Demo(props: { model: GoodsDto; count?: number }) {
  const { model, count } = props;

  return (
    <Box
      sx={{
        bgcolor: 'background.paper',
        overflow: 'hidden',
        borderRadius: '12px',
        boxShadow: 1,
        cursor: 'pointer',
        '&:hover': {
          border: (theme) => `1px solid ${theme.palette.primary.main}`,
        },
      }}
    >
      <XBase model={model} count={count} />
    </Box>
  );
}
