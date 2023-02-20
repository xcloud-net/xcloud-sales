export default {
  apiGateway: process.env.gatewayAddress || '',
  isDev: process.env.app_dev || false,
  appBase: process.env.app_base,
  app: {
    name: '豆芽美妆',
    slogan: '',
    version: '1.0.0',
    layout: {
      DRAWER_WIDTH: 280,
      APPBAR_MOBILE: 64,
      APPBAR_DESKTOP: 92,
    },
  },
  wx: {
    mp: {
      appid: 'wx05c5519078a8a369',
    },
  },
};
