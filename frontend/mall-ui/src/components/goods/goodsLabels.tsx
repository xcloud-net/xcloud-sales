import { Stack, Typography } from '@mui/material';
import Box from '@mui/material/Box';
import { GoodsDto } from '@/utils/models';

export default function Demo(props: { model: GoodsDto }) {
  const { model } = props;

  return (
    <Box sx={{}}>
      <Stack
        spacing={1}
        direction="row"
        alignItems={'center'}
        justifyContent="flex-start"
      >
        {model.StickyTop && (
          <Typography
            color="primary"
            variant="overline"
            sx={{
              display: 'inline',
            }}
          >
            置顶🔝
          </Typography>
        )}
        {model.IsNew && (
          <Typography
            color="primary"
            variant="overline"
            sx={{
              display: 'inline',
            }}
          >
            新品
          </Typography>
        )}
        {model.IsHot && (
          <Typography
            color="primary"
            variant="overline"
            sx={{
              display: 'inline',
            }}
          >
            热销🔥
          </Typography>
        )}
      </Stack>
    </Box>
  );
}
