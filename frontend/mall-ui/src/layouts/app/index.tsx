import XVConsole from '@/components/vconsole';
import { Box } from '@mui/material';
import { Toaster } from 'react-hot-toast';
import XLoginStatus from './loginStatus';
import XTheme from './theme';

export default function SimpleContainer(props: any) {
  const { children } = props;

  return (
    <>
      <XVConsole />
      <Toaster />
      <XTheme>
        <Box
          sx={{
            display: 'none',
          }}
        >
          <XLoginStatus />
        </Box>
        <Box sx={{}}>{children}</Box>
      </XTheme>
    </>
  );
}
