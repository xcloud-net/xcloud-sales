import u from '@/utils';
import { Settings } from '@mui/icons-material';
import AssignmentTurnedInIcon from '@mui/icons-material/AssignmentTurnedIn';
import BusinessIcon from '@mui/icons-material/Business';
import FavoriteIcon from '@mui/icons-material/Favorite';
import LocalActivityIcon from '@mui/icons-material/LocalActivity';
import MobileFriendlyIcon from '@mui/icons-material/MobileFriendly';
import { history } from 'umi';

import {
  Box,
  ListItemIcon,
  ListItemText,
  MenuItem,
  MenuList,
  Paper,
  Typography,
} from '@mui/material';
import { useEffect, useState } from 'react';

interface IMenu {
  icon?: any;
  name?: string;
  path?: string;
  action?: any;
  right?: any;
}

const index = function IconMenu(props: any) {
  const [pendingOrderCount, _pendingOrderCount] = useState(0);
  const [mobile, _mobile] = useState('');

  const queryUserMobile = () => {
    u.http.platformRequest.post('/user/mobile/mine', {}).then((res) => {
      if (res.data.Error) {
      } else {
        _mobile(res.data.Data);
      }
    });
  };

  const queryPendingOrderCount = () => {
    u.http.apiRequest.post('/mall/order/pending-count').then((res) => {
      u.handleResponse(res, () => {
        _pendingOrderCount(res.data.Data || 0);
      });
    });
  };

  useEffect(() => {
    queryPendingOrderCount();
    queryUserMobile();
  }, []);

  const actions: Array<IMenu> = [
    {
      icon: <AssignmentTurnedInIcon color="info" />,
      name: '订单',
      path: '/order',
      right: pendingOrderCount > 0 ? `${pendingOrderCount}个进行中` : ``,
    },
    {
      icon: <FavoriteIcon color="error" />,
      name: '愿望清单',
      path: '/favorites',
    },
    {
      icon: <LocalActivityIcon color="warning" />,
      name: '优惠券',
      path: '/user/coupon',
    },
  ];

  const actions2: Array<IMenu> = [
    {
      icon: <BusinessIcon />,
      name: '收货地址',
      right: '⌘X',
      path: '/user/address',
    },
    {
      icon: <MobileFriendlyIcon />,
      name: '绑定手机',
      right: u.isEmpty(mobile) ? '⌘C' : `${mobile}`,
      action: () => {},
    },
  ];

  const actions3: Array<IMenu> = [
    {
      icon: <Settings />,
      name: '设 置',
      path: '/ucenter/profile',
    },
  ];

  const renderMenu = (items: Array<any>) => {
    return (
      <Paper sx={{ maxWidth: '100%' }}>
        <MenuList>
          {u.map(items, (x) => (
            <MenuItem
              onClick={() => {
                if (x.path) {
                  history.push(x.path);
                } else {
                  x.action && x.action();
                }
              }}
            >
              <ListItemIcon>{x.icon}</ListItemIcon>
              <ListItemText>{x.name}</ListItemText>
              <Typography variant="body2" color="text.secondary">
                {x.right}
              </Typography>
            </MenuItem>
          ))}
        </MenuList>
      </Paper>
    );
  };

  return (
    <>
      <Box sx={{ mb: 3, px: 1 }}>
        <Box sx={{ mb: 1 }}>{renderMenu(actions)}</Box>
        <Box sx={{ mb: 1 }}>{renderMenu(actions2)}</Box>
        <Box sx={{ mb: 1 }}>{renderMenu(actions3)}</Box>
      </Box>
    </>
  );
};

export default index;
