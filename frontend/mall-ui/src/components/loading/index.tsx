import { Box } from '@mui/material';
import { SpinLoading } from 'antd-mobile';

export default function Animations() {
  return (
    <Box
      sx={{
        minHeight: 300,
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
      }}
    >
      <SpinLoading color={'default'} />
    </Box>
  );
}
