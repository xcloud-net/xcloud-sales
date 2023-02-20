
```ts
interface IRoute {
  title?: string;
  //for pro layout
  name?: string;
  icon?: string;
  path?: string;
  component?: string;
  exact?: boolean;
  redirect?: string;
  hideInMenu?: boolean;
  hideChildrenInMenu?: boolean;
  routes?: IRoute[];
  wrappers?: string[];
}

const adminRoutes: Array<IRoute> = [
  {
    path: '/manage/content/pages/preview',
    component: '@/layouts/app',
    routes: [
      {
        //预览页面，单独拎出来
        path: '/manage/content/pages/preview',
        component: './storeManage/pages/xpage/editor/previewPage',
      },
    ],
  },
  {
    name: 'manage',
    path: '/manage',
    component: './storeManage/layouts',
    //wrappers: ['@/layouts/app'],
    routes: [
      {
        path: '/manage',
        redirect: '/manage/dashboard',
        //redirect: '/manage/goods/list',
      },
      {
        path: '/manage/dashboard',
        routes: [
          {
            path: '/manage/dashboard',
            redirect: '/manage/dashboard/home',
          },
          {
            name: '统计报表',
            path: '/manage/dashboard/home',
            component: './storeManage/pages/dashboard/home',
          },
          {
            name: 'map',
            path: '/manage/dashboard/map',
            component: './storeManage/pages/dashboard/home/map',
          },
          {
            name: '销售报表',
            path: '/manage/dashboard/sales',
            component: './storeManage/pages/dashboard/sales',
          },
        ],
      },
      {
        name: '商品管理',
        path: '/manage/goods/list',
        component: './storeManage/pages/goods/list',
      },
      {
        name: '品牌管理',
        path: '/manage/brand/list',
        component: './storeManage/pages/brand/list',
      },
      {
        name: '分类管理',
        path: '/manage/category/list',
        component: './storeManage/pages/category/list',
      },
      {
        name: '标签管理',
        path: '/manage/tag/list',
        component: './storeManage/pages/tag/list',
      },
      {
        name: '会员等级',
        path: '/manage/user-grade',
        component: './storeManage/pages/user/grade',
      },
      {
        name: '商城会员',
        path: '/manage/users',
        component: './storeManage/pages/user/list',
      },
      {
        name: '门店管理',
        path: '/manage/store/list',
        component: './storeManage/pages/store/list',
      },
      {
        name: '内容管理',
        path: '/manage/content',
        routes: [
          {
            name: '内容管理',
            path: '/manage/content/pages',
            component: './storeManage/pages/xpage/list',
          },
        ],
      },
      {
        name: '订单管理',
        path: '/manage/order',
        routes: [
          {
            name: '订单列表',
            path: '/manage/order/list',
            component: './storeManage/pages/order/list',
          },
          {
            name: '订单详情',
            path: '/manage/order/detail/:id',
            component: './storeManage/pages/order/detail',
          },
        ],
      },
      {
        name: '营销方案',
        path: '/manage/marketing',
        routes: [
          {
            path: '/manage/marketing',
            redirect: '/manage/marketing/plan',
          },
          {
            name: '营销方案',
            path: '/manage/marketing/activity',
            component: './storeManage/pages/marketing/activity/list',
          },
          {
            name: '优惠券',
            path: '/manage/marketing/coupon',
            component: './storeManage/pages/marketing/coupon/list',
          },
          {
            name: '商品组合',
            path: '/manage/marketing/plan',
            component: './storeManage/pages/marketing/plan',
          },
          {
            name: '平行替换',
            path: '/manage/marketing/replace',
            component: './storeManage/pages/marketing/replace',
          },
        ],
      },
      {
        name: '商城设置',
        path: '/manage/settings',
        routes: [
          {
            name: '环境变量',
            path: '/manage/settings/env',
            component: './storeManage/pages/settings/env',
          },
        ],
      },
      {
        name: '系统设置',
        path: '/manage/sys',
        routes: [
          {
            path: '/manage/sys',
            redirect: '/manage/sys/user/list',
          },
          {
            name: '平台用户',
            path: '/manage/sys/user/list',
            component: './storeManage/pages/sys/user/list',
          },
          {
            name: '省市区',
            path: '/manage/sys/region',
            component: './storeManage/pages/sys/region',
          },
          {
            name: '租户',
            path: '/manage/sys/tenant',
            component: './storeManage/pages/sys/tenant/list',
          },
          {
            name: '系统管理员',
            path: '/manage/sys/administrator',
            component: './storeManage/pages/sys/administrator',
          },
        ],
      },
    ],
  },
];

const appRoutes: Array<IRoute> = [
  {
    path: '/',
    component: '@/layouts/app',
    routes: [
      {
        path: '/about',
        component: '@/components/aboutEntry',
        routes: [
          {
            title: '大会员',
            path: '/about/vip',
            component: './about/vip',
            wrappers: ['@/layouts/simpleHeader'],
          },
          {
            title: '服务细则',
            path: '/about/contract',
            component: './about/contract',
            wrappers: ['@/layouts/simpleHeader'],
          },
          {
            title: '关于',
            path: '/about',
            component: './about',
            wrappers: ['@/layouts/simpleHeader'],
          },
        ],
      },
      {
        path: '/account',
        component: '@/layouts/header',
        routes: [
          {
            title: '登录',
            path: '/account/login',
            component: './account/signIn',
          },
          {
            title: '注册',
            path: '/account/register',
            component: './account/register',
          },
        ],
      },
      {
        path: '/',
        component: '@/layouts/header',
        routes: [
          {
            title: '主题',
            path: '/pages',
            component: './xpage/list',
          },
          {
            title: '主题详情',
            path: '/pages/:id',
            component: './xpage/detail',
          },
          {
            title: '平行替换',
            path: '/replace',
            component: './replace',
          },
          {
            title: '方案',
            path: '/plan',
            component: './plan/list',
          },
          {
            title: '方案详情',
            path: '/plan/:id',
            component: './plan/detail',
          },
          {
            title: '商品类目',
            path: '/category',
            component: './category',
            wrappers: ['@/layouts/tab'],
          },
          {
            title: '品牌',
            path: '/brand',
            component: './brand',
          },
          {
            title: '商品标签',
            path: '/tags',
            component: './tags',
          },
          {
            title: '商品',
            path: '/goods',
            component: './goods/list',
            //wrappers: ['./goods/list/keepalive'],
          },
          {
            title: '商品详情',
            path: '/goods/:id',
            component: './goods/detail',
          },
          {
            title: '优惠券',
            path: '/coupon',
            component: './coupon',
          },
          {
            title: '豆芽家',
            path: '/',
            component: './index',
            wrappers: ['@/layouts/tab'],
          },
          {
            path: '/',
            component: '@/components/login/guide',
            routes: [
              {
                title: '个人资料',
                path: '/ucenter/profile',
                component: './user/profile',
              },
              {
                title: '个人中心',
                path: '/ucenter',
                component: './user/ucenter',
                wrappers: ['@/layouts/tab'],
              },
              {
                title: '愿望清单',
                path: '/favorites',
                component: './goods/favorites',
              },
              {
                title: '购物车',
                path: '/shoppingcart',
                component: './cart',
                wrappers: ['@/layouts/tab'],
              },
              {
                title: '结算中心',
                path: '/checkout',
                component: './cart/checkout',
              },
              {
                title: '订单',
                path: '/order',
                component: './order/list',
              },
              {
                title: '售后',
                path: '/aftersale',
                component: './aftersale/list',
              },
              {
                title: '订单详情',
                path: '/order/:id',
                component: './order/detail',
              },
              {
                title: '邮寄地址',
                path: '/user/address',
                component: './user/address',
              },
              {
                title: '通知',
                path: '/inbox',
                component: './inbox/list',
              },
            ],
          },
        ],
      },
    ],
  },
];

//能匹配越多信息的路由越靠前，避免覆盖低优先级的路由
const routes: Array<IRoute> = [
  {
    name: 'index',
    path: '/',
    component: '@/layouts',
    routes: [...adminRoutes, ...appRoutes],
  },
  {
    component: './err/404',
  },
];

export default routes;

```