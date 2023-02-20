import XLoginCallback from '@/components/login/auth';
import NotificationsIcon from '@mui/icons-material/Notifications';
import { Badge, IconButton } from '@mui/material';
import * as React from 'react';
import { history, useModel } from 'umi';

const index = function (props: any) {
  const storeAppAccountModel = useModel('storeAppAccount');
  const notificationModel = useModel('storeAppNotification');

  const queryNotificationCount = () => {
    if (!storeAppAccountModel.isUserLogin()) {
      return;
    }
    notificationModel.queryNotificationCount();
  };

  React.useEffect(() => {
    queryNotificationCount();
  }, []);

  const count = notificationModel.count || 0;

  return (
    <>
      <XLoginCallback
        onLogin={() => {
          //queryNotificationCount();
        }}
        onLogout={() => {
          notificationModel._count(0);
        }}
      />
      <IconButton
        size="large"
        color="inherit"
        onClick={() => {
          history.push('/inbox');
        }}
        sx={{}}
      >
        <Badge
          variant="dot"
          badgeContent={count}
          invisible={count <= 0}
          color="error"
        >
          <NotificationsIcon />
        </Badge>
      </IconButton>
    </>
  );
};

export default index;
