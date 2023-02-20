import * as React from 'react';
import storeAppService from '@/services/storeApp';

export default () => {
  const [count, _count] = React.useState(0);

  const queryShoppingcartCount = React.useCallback(async () => {
    var shoppingcartCount = await storeAppService.queryShoppingcartCount();
    _count(shoppingcartCount);
  }, []);

  return { count, _count, queryShoppingcartCount };
};
