import avatar_url from '@/assets/static/images/avatars/avatar_21.jpg';
import u from '@/utils';
import { Paper } from '@mui/material';

import { Avatar, Box, Typography } from '@mui/material';
import { useModel } from 'umi';
import XNotification from './components/notification';

const index = function (props: any) {
  const storeAppAccountModel = useModel('storeAppAccount');

  const renderUserChip = () => {
    if (!u.isEmpty(storeAppAccountModel.StoreUser?.GradeName)) {
      return (
        <Typography
          sx={{
            marginLeft: 8,
            display: 'inline-block',
            textTransform: 'uppercase',
            whiteSpace: 'nowrap',
            lineHeight: 1,
            fontWeight: 600,
            fontStyle: 'normal',
            marginInlineStart: '.66em',
            borderRadius: '.25em',
            padding: '.33em',
            color: '#000',
            backgroundColor: '#fec846',
          }}
          variant="overline"
          component={'em'}
        >
          {storeAppAccountModel.StoreUser?.GradeName}
        </Typography>
      );
    }

    return null;
  };

  return (
    <>
      <Paper
        sx={{
          mb: 3,
          pt: 5,
          pb: 3,
          px: 2,
          background: 'none',
        }}
        elevation={0}
      >
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'flex-start',
          }}
        >
          <Avatar
            variant="rounded"
            src={u.resolveAvatar(storeAppAccountModel.StoreUser.Avatar, {
              width: 80,
              height: 80,
            })}
            sx={{
              width: 80,
              height: 80,
            }}
          >
            <img src={avatar_url} />
          </Avatar>
          <Box sx={{ ml: 2 }}>
            <Typography gutterBottom variant="h5" component="div">
              {storeAppAccountModel.StoreUser?.NickName || '新用户'}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              <span>{`ID:@${storeAppAccountModel.StoreUser?.Id}`}</span>
              {renderUserChip()}
            </Typography>
          </Box>
          <Box
            sx={{
              width: '100%',
              display: 'flex',
              flexDirection: 'row',
              alignItems: 'center',
              justifyContent: 'flex-end',
            }}
          >
            <Box
              sx={{
                display: 'inline-block',
              }}
            >
              <XNotification />
            </Box>
          </Box>
        </Box>
      </Paper>
    </>
  );
};

export default index;
