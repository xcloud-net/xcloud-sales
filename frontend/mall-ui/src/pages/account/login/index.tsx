import loginPic from '@/assets/static/illustrations/illustration_login.png';
import u from '@/utils';
import http from '@/utils/http';
import { LoadingButton } from '@mui/lab';
import {
  Box,
  Button,
  Container,
  Paper,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import * as React from 'react';
import { history, useModel } from 'umi';
import XConnect from './connect';

export default function (props: any) {
  const [data, _data] = React.useState({
    IdentityName: '',
    Password: '',
  });
  const [loginLoading, _loginLoading] = React.useState(false);
  const next: any = history.location.query?.next || '/';
  const appSettings = useModel('storeAppSetting');

  React.useEffect(() => {
    localStorage.setItem('login_next', next);
    appSettings.queryMallSettings();
  }, []);

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (u.isEmpty(data.IdentityName) || u.isEmpty(data.Password)) {
      u.error('请输入完整的用户名和密码');
      return;
    }

    _loginLoading(true);
    http.platformRequest
      .post('/user/auth/password-login', data)
      .then((res) => {
        if (res.data.Error) {
          u.error(res.data.Error.Message);
        } else {
          u.setAccessToken(res.data.Data.AccessToken || '');

          u.success('登录成功');

          setTimeout(() => {
            console.log(next);
            window.location.href = next;
          }, 1000);
        }
      })
      .finally(() => {
        _loginLoading(false);
      });
  };

  const renderTopPic = () => {
    return (
      <Stack spacing={2} direction="column" alignItems={'center'}>
        <img src={loginPic} width={200} />
        <Typography component="h1" variant="h5">
          登录
        </Typography>
      </Stack>
    );
  };

  return (
    <>
      <Container maxWidth="sm" disableGutters>
        <Paper
          sx={{
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            p: 2,
          }}
          elevation={0}
        >
          {renderTopPic()}
          <Box
            component="form"
            onSubmit={handleSubmit}
            noValidate
            sx={{ mt: 1 }}
          >
            <TextField
              margin="normal"
              required
              fullWidth
              label="账号"
              name="email"
              value={data.IdentityName}
              onChange={(e) => _data({ ...data, IdentityName: e.target.value })}
              autoFocus
            />
            <TextField
              margin="normal"
              required
              fullWidth
              name="password"
              label="密码"
              type="password"
              value={data.Password}
              onChange={(e) => _data({ ...data, Password: e.target.value })}
              autoComplete="current-password"
            />
            <LoadingButton
              loading={loginLoading}
              type="submit"
              fullWidth
              variant="contained"
              sx={{ mt: 5, mb: 2 }}
            >
              登录
            </LoadingButton>
            <Button
              variant="text"
              fullWidth
              onClick={() => {
                history.push({
                  pathname: '/',
                });
              }}
            >
              返回
            </Button>
            <XConnect />
          </Box>
          {u.isEmpty(appSettings.mallSettings.LoginNotice) || (
            <Box sx={{ mt: 2 }}>
              <Typography variant="overline" color="primary">
                {appSettings.mallSettings.LoginNotice}
              </Typography>
            </Box>
          )}
        </Paper>
      </Container>
    </>
  );
}
