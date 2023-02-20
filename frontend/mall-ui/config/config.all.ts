// https://umijs.org/config/
import { defineConfig } from 'umi';

export default defineConfig({
  base: '/store/',
  publicPath: '/store/',
  define: {
    'process.env.app_dev': false,
  },
});
