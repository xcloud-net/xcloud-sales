import axios, { AxiosInstance } from 'axios';
import { history } from 'umi';
import _ from './lodash';
import utils from './utils';

const errorHandler = (error: any) => {
  const { response, request, config } = error;
  var currentPath = history.location.pathname;
  const url = config?.url;
  console.log(
    'errorHandler',
    error,
    response,
    request,
    config,
    url,
    currentPath,
  );

  if (!response) {
    alert('无法请求网络');
    return;
  }
  const status = response.status;

  if (status === 401) {
    utils.setAccessToken('');
  } else if (status == 403) {
    //
  } else {
    alert(`请求错误 ${status}: ${url}`);
  }
};

const responseHandler = (data: any) => {
  var error = data.Error;
  if (!error) {
    return;
  }
  console.error(error);
};

const getGlobalHeaders = () => {
  var customHeaders = {};

  var token = utils.getAccessToken();
  if (!_.isEmpty(token)) {
    customHeaders = { ...customHeaders, Authorization: `Bearer ${token}` };
  }

  return customHeaders;
};

const interceptRequest = (request: AxiosInstance) => {
  return request.interceptors.request.use(
    (request) => {
      var globalHeaders = getGlobalHeaders();
      request.headers = request.headers || {};
      request.headers = {
        ...globalHeaders,
        ...request.headers,
      };
      return request;
    },
    (err) => Promise.reject(err),
  );
};

const interceptResponse = (request: AxiosInstance) => {
  return request.interceptors.response.use(
    (response) => {
      response.data = response.data || {};
      responseHandler(response.data);

      return response;
    },
    (err) => {
      errorHandler(err);
      return Promise.reject(err);
    },
  );
};

const server = process.env.gatewayAddress;
if (_.isEmpty(server)) {
  console.error('请配置gatewayAddress');
}

const defaultRequest = axios.create({});

const adminRequest = axios.create({
  baseURL: `${server}/api/sys`,
});
interceptRequest(adminRequest);
interceptResponse(adminRequest);

const platformRequest = axios.create({
  baseURL: `${server}/api/platform`,
});
interceptRequest(platformRequest);
interceptResponse(platformRequest);

const apiRequest = axios.create({
  baseURL: `${server}/api`,
});
interceptRequest(apiRequest);
interceptResponse(apiRequest);

export default {
  defaultRequest,
  adminRequest,
  platformRequest,
  apiRequest,
};
