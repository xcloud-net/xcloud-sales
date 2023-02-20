import XMD from '@/components/markdown';
import u from '@/utils';
import { Box, Card, CardContent, Typography } from '@mui/material';
import { GoodsDto } from '@/utils/models';

export default (props: { model: GoodsDto }) => {
  const { model } = props;

  return (
    <>
      <Box sx={{}}>
        {u.isEmpty(model.FullDescription) || (
          <Card sx={{ mt: 1, borderRadius: 0 }}>
            <CardContent>
              <Typography variant="h6" component="div" gutterBottom>
                详情
              </Typography>
              <Box sx={{}}>
                <XMD md={model.FullDescription} />
              </Box>
            </CardContent>
          </Card>
        )}
      </Box>
    </>
  );
};
