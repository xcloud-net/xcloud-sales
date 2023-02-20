import { history } from 'umi';
import _ from './lodash';
import message from './message';
import { IPageLink } from './models';

const go = (data: IPageLink) => {
  if (data.goods && data.goods > 0) {
    history.push({
      pathname: `/goods/${data.goods}`,
    });
  } else if (!_.isEmpty(data.keywords)) {
    history.push({
      pathname: '/goods',
      query: {
        kwd: data.keywords || '',
      },
    });
  } else if (data.path) {
    history.push({
      pathname: data.path.pathname,
      query: data.path.query || {},
    });
  } else {
    message.error('配置错误');
  }
};

export default {
  go,
};
