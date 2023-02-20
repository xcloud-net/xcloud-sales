import { Stack } from '@mui/material';
import Alert from '@mui/material/Alert';
import AlertTitle from '@mui/material/AlertTitle';

export default function DescriptionAlerts() {
  return (
    <Stack sx={{ width: '100%' }} spacing={2}>
      <Alert severity="warning">
        <AlertTitle>暂无数据</AlertTitle>
        <p>模块建设中，详情请咨询系统管理员</p>
      </Alert>
    </Stack>
  );
}
