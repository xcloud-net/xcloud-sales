// https://umijs.org/config/
import { defineConfig } from 'umi';
import routes from './routes';
import { first, filter, isEmpty } from 'lodash';

//drop manage routes
const dropManageRoutes = () => {
  var index = first(filter(routes, (x) => x.name == 'index')) || {};
  var others = filter(routes, (x) => x.name != 'index');

  var newIndex = {
    ...index,
    routes: [...filter(index.routes, (x) => x.name != 'manage')],
  };

  var newRoutes = [newIndex, ...others];
  //console.log(JSON.stringify(newRoutes));
  console.log('drop management routes');
  return newRoutes;
};

const APP_BASE = process.env.APP_BASE || '/store/';
const API_ADDRESS_FROM_ENV = process.env.gatewayAddress;
const FINAL_API_ADDRESS = isEmpty(API_ADDRESS_FROM_ENV)
  ? 'https://your-dev-domain.com:8888'
  : API_ADDRESS_FROM_ENV;

console.log('API网关地址：', FINAL_API_ADDRESS);

export default defineConfig({
  hash: true,
  base: APP_BASE,
  publicPath: APP_BASE,
  //runtimePublicPath: true,
  routes: process.env.XROUTES == 'app' ? dropManageRoutes() : routes,
  manifest: {
    basePath: '/',
  },
  define: {
    'process.env.UMI_ENV': process.env.UMI_ENV,
    'process.env.gatewayAddress': FINAL_API_ADDRESS,
    'process.env.app_dev': true,
    'process.env.app_base': APP_BASE,
  },
  dynamicImport: {
    loading: '@/components/loading',
  },
  analytics: {
    baidu: '9c58865284c9aa492d2c7d890ef0b35f',
  },
  antd: false,
  request: false,
  analyze: {
    analyzerMode: 'server',
    analyzerPort: 4444,
    openAnalyzer: true,
    // generate stats file while ANALYZE_DUMP exist
    generateStatsFile: false,
    statsFilename: 'stats.json',
    logLevel: 'info',
    defaultSizes: 'parsed', // stat  // gzip
  },
});
