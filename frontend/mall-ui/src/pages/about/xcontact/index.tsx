import { Box, Typography } from '@mui/material';
import XWebsite from './website';
import XWechat from './wechat';

export default () => {
  return (
    <>
      <Box sx={{ mb: 3, px: 1, py: 1, backgroundColor: 'white' }}>
        <Typography variant="h3" component={'div'} gutterBottom>
          保持联系
        </Typography>
        <XWechat />
        <XWebsite />
      </Box>
    </>
  );
};
