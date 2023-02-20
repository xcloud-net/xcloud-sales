import u from '@/utils';
import { GoodsDto } from '@/utils/models';
import { Box, Chip, Stack } from '@mui/material';

const index = function (props: { model: GoodsDto; children: any }) {
  const { model, children } = props;

  if (!model || !model.Id) {
    return null;
  }

  var tags = u.take(model.Tags || [], 1);

  return (
    <>
      <Box
        sx={{
          position: 'relative',
          overflow: 'hidden',
        }}
      >
        {children}
        <Box sx={{ position: 'absolute', top: 10, left: 10, zIndex: 100 }}>
          <Stack direction="row" alignItems="center" spacing={1}>
            {u.map(tags || [], (x: any, index) => {
              return (
                <Chip
                  key={index}
                  label={x.Name}
                  color="secondary"
                  size="small"
                />
              );
            })}
          </Stack>
        </Box>
      </Box>
    </>
  );
};

export default index;
