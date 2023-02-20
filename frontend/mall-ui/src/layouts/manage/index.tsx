import { Box } from '@mui/material';
import { ConfigProvider } from 'antd';
import zhCN from 'antd/es/locale/zh_CN';
import { useEffect } from 'react';
import { Toaster } from 'react-hot-toast';
import XLayout from './layout';
import XLoginStatus from './loginStatus';
import XMenu from './menu';

export default function App(props: any) {
  const { children } = props;

  useEffect(() => {
    //
  }, []);

  return (
    <ConfigProvider locale={zhCN} componentSize="small">
      <Box
        sx={{
          display: 'none',
        }}
      >
        <XLoginStatus />
      </Box>
      <Toaster />
      <XLayout>
        <XMenu>{children}</XMenu>
      </XLayout>
    </ConfigProvider>
  );
}
