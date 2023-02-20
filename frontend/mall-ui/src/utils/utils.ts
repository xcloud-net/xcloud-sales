import _ from './lodash';
import message from './message';
import { AxiosResponse } from 'axios';

const handleResponse = (res: AxiosResponse, callback: any) => {
  if (res.data.Error) {
    message.error(res.data.Error.Message || '操作未能如期完成');
    return false;
  } else {
    callback && callback();
    return true;
  }
};

const concatUrl = (paths: Array<string>) => {
  var list: Array<string> = [];
  paths.forEach((x) => {
    if (!_.isEmpty(x)) {
      list.push(_.trim(x, '/'));
    }
  });

  list = _.filter(list, (x) => !_.isEmpty(x));

  return _.join(list, '/');
};

const normalizePath = (path: string) => {
  path = _.lowerCase(path);
  path = _.trim(path, '/');
  return path;
};

const pathEqual = (path1: string, path2: string) => {
  path1 = path1 || '';
  path2 = path2 || '';
  return normalizePath(path1) === normalizePath(path2);
};

const setAccessToken = (token: any) =>
  localStorage.setItem('access_token', token);
const getAccessToken = () => localStorage.getItem('access_token');

const hasAccessToken = () => !_.isEmpty(getAccessToken());

const setTenant = (t: any) => localStorage.setItem('tenant', t);
const getTenant = () => localStorage.getItem('tenant');

const getFileExtension = (filename: string) => {
  var extension = _.last(_.split(filename || '', '.'));
  return extension;
};

const isMobile = (mobile: string) => {
  return !_.isEmpty(mobile) && /^(?:(?:\+|00)86)?1[3-9]\d{9}$/.test(mobile);
};

const isEmail = (email: string) => {
  return (
    !_.isEmpty(email) &&
    /^[A-Za-z0-9\u4e00-\u9fa5]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$/.test(email)
  );
};

const isIdentityCard = (idno: string) => {
  return (
    !_.isEmpty(idno) &&
    /^\d{6}((((((19|20)\d{2})(0[13-9]|1[012])(0[1-9]|[12]\d|30))|(((19|20)\d{2})(0[13578]|1[02])31)|((19|20)\d{2})02(0[1-9]|1\d|2[0-8])|((((19|20)([13579][26]|[2468][048]|0[48]))|(2000))0229))\d{3})|((((\d{2})(0[13-9]|1[012])(0[1-9]|[12]\d|30))|((\d{2})(0[13578]|1[02])31)|((\d{2})02(0[1-9]|1\d|2[0-8]))|(([13579][26]|[2468][048]|0[048])0229))\d{2}))(\d|X|x)$/.test(
      idno,
    )
  );
};

const simpleString = (value: string, len: number) => {
  if (value.length <= len) {
    return value;
  }
  return `${value.substring(0, len)}...`;
};

const firstNotEmpty = (data: (string | undefined)[]) => {
  var item = _.first(data.filter((x) => !_.isEmpty(x)));
  return item;
};

export default {
  firstNotEmpty,
  simpleString,
  formatMoney: (value: number) => {
    return Number(value).toFixed(2);
  },
  getFileExtension,
  copyData: (data: any) => JSON.parse(JSON.stringify(data)),
  normalizePath,
  pathEqual,
  setAccessToken,
  getAccessToken,
  hasAccessToken,
  setTenant,
  getTenant,
  concatUrl,
  handleResponse,
  validator: {
    isMobile,
    isEmail,
    isIdentityCard,
  },
};
