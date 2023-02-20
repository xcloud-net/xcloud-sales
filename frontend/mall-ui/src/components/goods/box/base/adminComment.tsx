import { GoodsDto } from '@/utils/models';
import { Box, Typography } from '@mui/material';

const index = function (props: { model: GoodsDto }) {
  const { model } = props;

  if (!model || !model.Id) {
    return null;
  }

  return (
    <>
      <Box
        sx={{
          display: {
            xs: 'none',
            sm: 'none',
            md: 'block',
          },
        }}
      >
        {model.AdminComment && (
          <Typography variant="overline" color="primary" display="block">
            {`${model.AdminComment}`}
          </Typography>
        )}
      </Box>
    </>
  );
};

export default index;
