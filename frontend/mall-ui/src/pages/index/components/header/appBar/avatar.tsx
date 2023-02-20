import avatar_url from '@/assets/static/images/avatars/avatar_21.jpg';
import u from '@/utils';
import { AccountCircleOutlined } from '@mui/icons-material';
import { Avatar, Badge, IconButton } from '@mui/material';
import { useEffect } from 'react';

import { history, useModel } from 'umi';

const index = function (props: any) {
  const storeAppAccountModel = useModel('storeAppAccount');
  const storeAppNotification = useModel('storeAppNotification');
  const appShoppingCartModel = useModel('storeAppShoppingcart');

  useEffect(() => {
    if (storeAppAccountModel.isUserLogin()) {
      storeAppNotification.queryNotificationCount();
      appShoppingCartModel.queryShoppingcartCount();
    }
  }, [storeAppAccountModel.StoreUser]);

  const badgeShow =
    storeAppNotification.count > 0 || appShoppingCartModel.count > 0;

  return (
    <>
      <IconButton
        edge="end"
        onClick={() => {
          history.push('/ucenter');
        }}
        color="inherit"
      >
        {storeAppAccountModel.isUserLogin() && (
          <Badge
            variant="dot"
            color="error"
            badgeContent={storeAppNotification.count}
            invisible={!badgeShow}
          >
            <Avatar
              sx={{
                border: (theme) => `2px solid ${theme.palette.grey[200]}`,
              }}
              src={u.resolveAvatar(storeAppAccountModel.StoreUser.Avatar, {
                width: 80,
                height: 80,
              })}
            >
              <img src={avatar_url} />
            </Avatar>
          </Badge>
        )}
        {storeAppAccountModel.isUserLogin() || <AccountCircleOutlined />}
      </IconButton>
    </>
  );
};

export default index;
