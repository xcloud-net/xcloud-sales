import XEmptyImage from '@/assets/static/illustrations/illustration_register.png';
import { Alert, Box } from '@mui/material';

export default function AlignItemsList() {
  return (
    <>
      <Box sx={{ margin: 2 }}>
        <Box sx={{ fontSize: 100, textAlign: 'center' }}>
          <img src={XEmptyImage} alt="" />
        </Box>
        <Alert severity="info">暂无数据</Alert>
      </Box>
    </>
  );
}
