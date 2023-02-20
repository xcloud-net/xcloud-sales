import * as React from 'react';
import { useLocation, useModel } from 'umi';
import u from '@/utils';

const index = function (props: any) {
  const storeAppAccountModel = useModel('storeAppAccount');
  const cacheKey = 'login-admin-flag';

  const queryProfile: any = async () => {
    await storeAppAccountModel.queryAdminProfile();
  };

  const refreshLoginStatus = () => {
    return !storeAppAccountModel.cacheIsValid(cacheKey);
  };

  const tryQueryProfile = async () => {
    if (storeAppAccountModel.isAdminLogin() && u.hasAccessToken()) {
      if (refreshLoginStatus()) {
        await queryProfile();
      }
    } else {
      await queryProfile();
    }
  };

  const location = useLocation();

  React.useEffect(() => {
    tryQueryProfile();
  }, [location.pathname]);

  return <></>;
};

export default index;
