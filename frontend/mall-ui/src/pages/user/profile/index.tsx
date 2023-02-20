import u from '@/utils';
import { Box, Button, Container } from '@mui/material';
import { useEffect, useState } from 'react';
import { history, useModel } from 'umi';
import XAvatar from './avatar';
import XIdentityName from './identityname';
import XItem from './item';
import XMobile from './mobile';
import XNickname from './nickname';
import LinearProgress from '@/components/loading/linear';

export default function Animations() {
  const storeAppAccountModel = useModel('storeAppAccount');
  const [model, _model] = useState<any>({});
  const [loading, _loading] = useState(false);

  const queryProfile = () => {
    _loading(true);
    u.http.platformRequest
      .post('/user/profile', {})
      .then((res) => {
        _model(res.data.Data || {});
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryProfile();
  }, []);

  return (
    <Container maxWidth="sm" disableGutters>
      {loading && <LinearProgress />}
      <Box
        sx={{
          backgroundColor: 'white',
          borderRadius: 1,
          overflow: 'hidden',
          mb: 2,
        }}
      >
        <XAvatar
          model={model}
          ok={() => {
            queryProfile();
          }}
        />
        <XIdentityName
          model={model}
          ok={() => {
            queryProfile();
          }}
        />
        <XNickname
          model={model}
          ok={() => {
            queryProfile();
          }}
        />
        <XMobile
          model={model}
          ok={() => {
            queryProfile();
          }}
        />
      </Box>
      <Box
        sx={{
          backgroundColor: 'white',
          borderRadius: 1,
          overflow: 'hidden',
          mb: 2,
        }}
      >
        <Box
          onClick={() => {
            //
          }}
        >
          <XItem title={'服务细则'} right={null} />
        </Box>
        <Box
          onClick={() => {
            history.push({
              pathname: '/about',
            });
          }}
        >
          <XItem title={'关于我们'} right={null} />
        </Box>
      </Box>
      {storeAppAccountModel.isUserLogin() && (
        <Box sx={{ margin: 2, mt: 4 }}>
          <Button
            fullWidth
            variant="contained"
            color="error"
            onClick={() => {
              if (confirm('确定退出嘛？')) {
                u.setAccessToken('');
                history.push({
                  pathname: '/',
                });
              }
            }}
          >
            退出登录
          </Button>
        </Box>
      )}
    </Container>
  );
}
