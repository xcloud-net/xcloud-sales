import * as React from 'react';

import {
  AllInclusiveOutlined,
  FolderSpecialOutlined,
  PermIdentityOutlined,
  SearchOutlined,
  ShoppingBagOutlined,
} from '@mui/icons-material';

import { useSize } from 'ahooks';
import { history, useLocation, useModel } from 'umi';

import {
  Badge,
  BottomNavigation,
  BottomNavigationAction,
  Box,
  Paper,
  styled,
} from '@mui/material';

const XBottomNavigationAction = styled(BottomNavigationAction)((theme) => ({
  '&.Mui-selected': {
    color: '#282b31',
  },
}));

export default function IndexPage(props: { children: any }) {
  const { children } = props;
  const [value, setValue] = React.useState('--');

  const location = useLocation();
  const appSettingModel = useModel('storeAppSetting');
  const appShoppingCartModel = useModel('storeAppShoppingcart');
  const appAccountModel = useModel('storeAppAccount');

  const ref = React.useRef(null);
  const rect = useSize(ref);

  React.useEffect(() => {
    if (appAccountModel.isUserLogin()) {
      appShoppingCartModel.queryShoppingcartCount();
    }
  }, [appAccountModel.StoreUser]);

  React.useEffect(() => {
    appSettingModel._bottomHeight(rect?.height || 0);
  }, [rect]);

  React.useEffect(() => {
    setValue(location.pathname);
  }, [location.pathname]);

  return (
    <>
      <Box sx={{ pb: `${appSettingModel.bottomHeight + 15}px` }}>
        {children}
      </Box>
      <Paper
        ref={ref}
        sx={{ position: 'fixed', bottom: 0, left: 0, right: 0, zIndex: 1 }}
        elevation={3}
      >
        <BottomNavigation
          showLabels
          value={value}
          onChange={(event, newValue) => {
            //setValue(newValue);
            history.push(newValue);
          }}
        >
          <XBottomNavigationAction
            value={'/'}
            label="豆芽家"
            icon={<AllInclusiveOutlined fontSize="small" />}
          />
          <XBottomNavigationAction
            value={'/category'}
            label="商品"
            icon={<FolderSpecialOutlined fontSize="small" />}
          />
          <XBottomNavigationAction
            value={'/shoppingcart'}
            label="购物袋"
            icon={
              <Badge
                variant="dot"
                color="error"
                invisible={appShoppingCartModel.count <= 0}
                badgeContent={appShoppingCartModel.count}
              >
                <ShoppingBagOutlined fontSize="small" />
              </Badge>
            }
          />
          <XBottomNavigationAction
            value={'/ucenter'}
            label="我的"
            icon={<PermIdentityOutlined fontSize="small" />}
            sx={{
              display: 'none',
            }}
          />
          <XBottomNavigationAction
            value={'/search'}
            label="搜索"
            icon={<SearchOutlined fontSize="small" />}
          />
        </BottomNavigation>
      </Paper>
    </>
  );
}
