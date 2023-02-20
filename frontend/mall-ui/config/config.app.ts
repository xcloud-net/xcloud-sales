// https://umijs.org/config/
import { defineConfig } from 'umi';

export default defineConfig({
  define: {
    'process.env.app_dev': false,
  },
  dynamicImport: {
    loading: '@/components/loading',
  },
});
