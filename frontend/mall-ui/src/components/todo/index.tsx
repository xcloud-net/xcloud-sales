import Alert from '@mui/material/Alert';
import AlertTitle from '@mui/material/AlertTitle';
import Stack from '@mui/material/Stack';
import { history } from 'umi';

export default function DescriptionAlerts() {
  return (
    <Stack sx={{ width: '100%' }} spacing={2}>
      <Alert severity="warning">
        <AlertTitle>提示</AlertTitle>
        功能开发中，敬请期待。 —{' '}
        <strong
          onClick={() => {
            history.push({
              pathname: '/',
            });
          }}
        >
          返回首页!
        </strong>
      </Alert>
    </Stack>
  );
}
