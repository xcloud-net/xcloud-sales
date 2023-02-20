import { ShoppingBagOutlined } from '@mui/icons-material';
import { Badge, IconButton } from '@mui/material';
import * as React from 'react';
import { history, useModel } from 'umi';

const index = function (props: any) {
  const storeAppAccountModel = useModel('storeAppAccount');
  const shoppingcartModel = useModel('storeAppShoppingcart');

  const queryShoppingcartCount = async () => {
    if (!storeAppAccountModel.isUserLogin()) {
      return;
    }

    await shoppingcartModel.queryShoppingcartCount();
  };

  React.useEffect(() => {
    queryShoppingcartCount();
  }, []);

  const count = shoppingcartModel.count || 0;

  if (count <= 0) {
    return null;
  }

  return (
    <>
      <IconButton
        color="inherit"
        sx={{}}
        onClick={() => {
          history.push('/shoppingcart');
        }}
      >
        <Badge badgeContent={count} invisible={count <= 0} color="error">
          <ShoppingBagOutlined />
        </Badge>
      </IconButton>
    </>
  );
};

export default index;
