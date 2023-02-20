import dayjs from './dayjs';
import storage from './storage';
import _ from './lodash';
import utils from './utils';
import antd from './antd';
import config from './config';
import umi from './umi';
import http from './http';
import link from './link';
import message from './message';
import color from './color';

export default {
  ...umi,
  ...antd,
  ...dayjs,
  ...utils,
  ...storage,
  ...link,
  ...message,
  ..._,
  color: color,
  config: config,
  http: http,
};
