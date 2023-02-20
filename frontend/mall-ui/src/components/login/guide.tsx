import LoginImage from '@/assets/static/illustrations/illustration_register.png';
import u from '@/utils';
import AccountBoxIcon from '@mui/icons-material/AccountBox';
import {
  Alert,
  AlertTitle,
  Box,
  Button,
  Container,
  Stack,
} from '@mui/material';
import XCallback from './auth';
import { history } from 'umi';
import LinearProgress from '@/components/loading/linear';

export default (props: any) => {
  const { children } = props;

  return (
    <XCallback
      loading={(x: any) => <LinearProgress />}
      unauth={(x: any) => (
        <Container maxWidth="sm" disableGutters>
          <Box sx={{ m: 2 }}>
            <Stack sx={{ width: '100%' }} spacing={2}>
              <Box sx={{ mb: 2, display: 'flex', justifyContent: 'center' }}>
                <img src={LoginImage} height={200} />
              </Box>
              <Alert severity="info">
                <AlertTitle>未登录</AlertTitle>
                登录可以开启完整功能！
              </Alert>

              <Box sx={{ margin: 2 }}>
                <Button
                  fullWidth
                  variant="contained"
                  color="primary"
                  startIcon={<AccountBoxIcon />}
                  onClick={() => {
                    u.redirectToLogin();
                  }}
                >
                  立马登录
                </Button>
              </Box>
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'row',
                  alignItems: 'center',
                  justifyContent: 'center',
                  pt: 3,
                }}
              >
                <Button
                  onClick={() => {
                    history.push({
                      pathname: '/',
                    });
                  }}
                >
                  返回首页
                </Button>
              </Box>
            </Stack>
          </Box>
        </Container>
      )}
    >
      {children}
    </XCallback>
  );
};
