import u from '@/utils';
import { Box, Typography, Container } from '@mui/material';
import { useEffect, useState } from 'react';
import { history } from 'umi';
import LinearProgress from '@/components/loading/linear';

export default () => {
  const [loading, _loading] = useState(false);

  const loginAction = (code: string) => {
    _loading(true);

    const usewechatprofileKey = 'usewechatprofile';
    const updateprofile = localStorage.getItem(usewechatprofileKey) == 'true';
    localStorage.removeItem(usewechatprofileKey);

    u.http.apiRequest
      .post('/platform/user/wx-auth/wx-mp-code-auth', {
        Code: code,
        UseWechatProfile: updateprofile,
      })
      .then((res) => {
        if (res.data.Error) {
          u.error(res.data.Error.Message || '操作未能如期完成');
          return false;
        } else {
          const { AccessToken } = res.data.Data || {};
          if (u.isEmpty(AccessToken)) {
            u.error('无法获取token');
            return;
          } else {
            u.setAccessToken(AccessToken);
            u.success('登录成功，即将跳转');
            var next = localStorage.getItem('login_next') || '/';
            setTimeout(() => {
              window.location.href = next;
            }, 500);
          }
          return true;
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    console.log(history);
    const { code } = history.location.query || {};
    if (u.isEmpty(code)) {
      history.push({
        pathname: '/',
      });
    } else {
      loginAction(code as string);
    }
  }, []);

  return (
    <>
      <Container maxWidth="sm" disableGutters>
        <Box sx={{}}>
          {loading && <LinearProgress />}
          <Box
            sx={{
              py: 4,
              display: 'flex',
              flexDirection: 'row',
              alignItems: 'center',
              justifyContent: 'center',
            }}
          >
            <Typography variant="h5">登录中...</Typography>
          </Box>
        </Box>
      </Container>
    </>
  );
};
