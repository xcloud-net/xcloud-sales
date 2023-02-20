import _ from './lodash';
import { history } from 'umi';
import qs from 'qs';

const redirectToLogin = () => {
  const queryString = qs.stringify(history.location.query || {}, {
    addQueryPrefix: false,
    skipNulls: true,
  });
  const currentPath = window.location.href;
  history.push({
    pathname: '/account/login',
    query: {
      next: currentPath,
    },
  });
};

export default {
  redirectToLogin,
};
