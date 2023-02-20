import u from '@/utils';
import { Alert, Box } from '@mui/material';
import { useEffect } from 'react';
import { useModel } from 'umi';

export default () => {
  const appSettings = useModel('storeAppSetting');

  useEffect(() => {
    appSettings.queryMallSettings();
  }, []);

  if (u.isEmpty(appSettings.mallSettings.GoodsDetailNotice)) {
    return null;
  }

  return (
    <>
      <Box sx={{ mb: 3 }}>
        <Alert>{appSettings.mallSettings.GoodsDetailNotice}</Alert>
      </Box>
    </>
  );
};
