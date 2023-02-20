import u from '@/utils';
import { Alert, Box } from '@mui/material';
import { useModel } from 'umi';

export default () => {
  const appSettings = useModel('storeAppSetting');

  if (u.isEmpty(appSettings.mallSettings.HomePageNotice)) {
    return null;
  }

  return (
    <>
      <Box
        sx={{
          px: 1,
          mb: 3,
        }}
      >
        <Alert>{appSettings.mallSettings.HomePageNotice}</Alert>
      </Box>
    </>
  );
};
