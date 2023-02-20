import { useEffect } from 'react';
import { useModel } from 'umi';

export default (props: any) => {
  const { children, loading, unauth, onLogin, onLogout } = props;
  const storeAppAccountModel = useModel('storeAppAccount');

  useEffect(() => {
    if (storeAppAccountModel.isUserLogin()) {
      onLogin && onLogin(storeAppAccountModel.StoreUser);
    } else {
      onLogout && onLogout();
    }
  }, [storeAppAccountModel.StoreUser]);

  if (!storeAppAccountModel.StoreUserLoaded) {
    return (loading && loading(children)) || null;
  }

  if (!storeAppAccountModel.isUserLogin()) {
    return (unauth && unauth(children)) || null;
  }

  return <>{children}</>;
};
