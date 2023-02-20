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
        component: './manage/settings/home/previewPage',
      },
    ],
  },
  {
    name: 'manage',
    path: '/manage',
    component: '@/layouts/manage',
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
            component: './manage/dashboard/home',
          },
          {
            name: '销售报表',
            path: '/manage/dashboard/sales',
            component: './manage/dashboard/sales',
          },
          {
            name: '搜索关键词',
            path: '/manage/dashboard/keywords',
            component: './manage/dashboard/keywords',
          },
          {
            name: '用户活动',
            path: '/manage/dashboard/activity',
            component: './manage/dashboard/activity',
          },
          {
            name: '活跃城市',
            path: '/manage/dashboard/activity-geo',
            component: './manage/dashboard/activity/geo',
          },
        ],
      },
      {
        name: '商品管理',
        path: '/manage/goods/list',
        component: './manage/catalog/goods/goods',
      },
      {
        name: '商品规格明细',
        path: '/manage/goods/combination',
        component: './manage/catalog/goods/combination',
      },
      {
        name: '品牌管理',
        path: '/manage/brand/list',
        component: './manage/catalog/brand/list',
      },
      {
        name: '分类管理',
        path: '/manage/category/list',
        component: './manage/catalog/category/list',
      },
      {
        name: '标签管理',
        path: '/manage/tag/list',
        component: './manage/catalog/tag/list',
      },
      {
        name: '会员等级',
        path: '/manage/user-grade',
        component: './manage/settings/malluser/grade',
      },
      {
        name: '商城会员',
        path: '/manage/users',
        component: './manage/settings/malluser/list',
      },
      {
        name: '门店管理',
        path: '/manage/store/list',
        component: './manage/settings/store/list',
      },
      {
        name: '预售充值卡',
        path: '/manage/prepaidcard/list',
        component: './manage/prepaidcard/list',
      },
      {
        name: '财务',
        path: '/manage/finance',
        routes: [
          {
            name: '订单支付单',
            path: '/manage/finance/order-bill',
            component: './manage/finance/orderBill',
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
            component: './manage/order/list',
          },
        ],
      },
      {
        name: '售后管理',
        path: '/manage/aftersales',
        routes: [
          {
            name: '售后列表',
            path: '/manage/aftersales/list',
            component: './manage/order/aftersale_list',
          },
        ],
      },
      {
        name: '仓库管理',
        path: '/manage/stock',
        routes: [
          {
            name: '采购单',
            path: '/manage/stock/stock',
            component: './manage/stock/stock/stock',
          },
          {
            name: '商品库存',
            path: '/manage/stock/stock-items',
            component: './manage/stock/stock/stock-item',
          },
          {
            name: '仓库管理',
            path: '/manage/stock/warehouse',
            component: './manage/stock/warehouse',
          },
          {
            name: '供应商管理',
            path: '/manage/stock/supplier',
            component: './manage/stock/supplier',
          },
        ],
      },
      {
        name: '营销方案',
        path: '/manage/marketing',
        routes: [
          {
            path: '/manage/marketing',
            redirect: '/manage/marketing/collection',
          },
          {
            name: '营销方案',
            path: '/manage/marketing/promotion',
            component: './manage/marketing/promotion/list',
          },
          {
            name: '优惠券',
            path: '/manage/marketing/coupon',
            component: './manage/marketing/coupon/list',
          },
          {
            name: '商品组合',
            path: '/manage/marketing/collection',
            component: './manage/marketing/collection',
          },
        ],
      },
      {
        name: '系统设置',
        path: '/manage/settings',
        routes: [
          {
            path: '/manage/settings',
            redirect: '/manage/settings/mall',
          },
          {
            name: '角色',
            path: '/manage/settings/roles',
            component: './manage/settings/role',
          },
          {
            name: '环境变量',
            path: '/manage/settings/env',
            component: './manage/settings/env',
          },
          {
            name: '首页设计',
            path: '/manage/settings/home-design',
            component: './manage/settings/home',
          },
          {
            name: '商城设置',
            path: '/manage/settings/mall',
            component: './manage/settings/mall',
          },
          {
            name: '平台用户',
            path: '/manage/settings/user/list',
            component: './manage/settings/user/list',
          },
          {
            name: '省市区',
            path: '/manage/settings/region',
            component: './manage/settings/region',
          },
          {
            name: '系统管理员',
            path: '/manage/settings/administrator',
            component: './manage/settings/administrator',
          },
          {
            name: '活动日志',
            path: '/manage/settings/log',
            component: './manage/settings/activityLog',
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
        //component: '@/components/aboutEntry',
        routes: [
          {
            title: '关于',
            path: '/about',
            component: './about',
            wrappers: ['@/layouts/app/tab'],
          },
        ],
      },
      {
        path: '/account',
        routes: [
          {
            title: '登录',
            path: '/account/login',
            component: './account/login',
          },
          {
            title: '微信回调',
            path: '/account/wx-callback',
            component: './account/login/wxcallback',
          },
          {
            title: '注册',
            path: '/account/register',
            component: './account/register',
          },
        ],
      },
      {
        title: '订单',
        path: '/order',
        component: './order/list',
        wrappers: ['@/components/login/guide', '@/layouts/app/simpleHeader'],
      },
      {
        title: '充值卡',
        path: '/ucenter/prepaidcard/:id',
        component: './user/prepaidcard',
        wrappers: ['@/components/login/guide', '@/layouts/app/simpleHeader'],
      },
      {
        title: '账户余额',
        path: '/ucenter/balance',
        component: './user/balance',
        wrappers: ['@/components/login/guide', '@/layouts/app/simpleHeader'],
      },
      {
        title: '积分',
        path: '/ucenter/points',
        component: './user/points',
        wrappers: ['@/components/login/guide', '@/layouts/app/simpleHeader'],
      },
      {
        title: '个人资料',
        path: '/ucenter/profile',
        component: './user/profile',
        wrappers: ['@/components/login/guide', '@/layouts/app/simpleHeader'],
      },
      {
        title: '邮寄地址',
        path: '/user/address',
        component: './user/address',
        wrappers: ['@/components/login/guide', '@/layouts/app/simpleHeader'],
      },
      {
        title: '愿望清单',
        path: '/favorites',
        component: './goods/favorites',
        wrappers: ['@/components/login/guide', '@/layouts/app/simpleHeader'],
      },
      {
        title: '我的优惠券',
        path: '/user/coupon',
        component: './user/coupon',
        wrappers: ['@/components/login/guide', '@/layouts/app/simpleHeader'],
      },
      {
        title: '购物车',
        path: '/shoppingcart',
        component: './cart',
        wrappers: [
          '@/layouts/app/tab',
          '@/components/login/guide',
          //'@/layouts/app/simpleHeader',
        ],
      },
      {
        title: '通知',
        path: '/inbox',
        component: './inbox/list',
        wrappers: ['@/components/login/guide', '@/layouts/app/simpleHeader'],
      },
      {
        title: '商品类目',
        path: '/category',
        component: './category',
        wrappers: ['@/layouts/app/tab'],
      },
      {
        title: '品牌',
        path: '/brand',
        component: './brand',
        wrappers: ['@/layouts/app/tab', '@/layouts/app/simpleHeader'],
      },
      {
        title: '搜索',
        path: '/search',
        component: './search',
        wrappers: ['@/layouts/app/tab', '@/layouts/app/simpleHeader'],
      },
      {
        title: '个人中心',
        path: '/ucenter',
        component: './user/ucenter',
        wrappers: ['@/layouts/app/tab', '@/components/login/guide'],
      },
      {
        title: '商品',
        path: '/goods',
        component: './goods/list',
        wrappers: ['@/layouts/app/tab', '@/layouts/app/simpleHeader'],
      },
      {
        title: '商品详情',
        path: '/goods/:id',
        component: './goods/detail',
        wrappers: ['@/layouts/app/tab', '@/layouts/app/simpleHeader'],
      },
      {
        path: '/',
        routes: [
          {
            title: '豆芽家',
            path: '/',
            component: './index',
            wrappers: ['@/layouts/app/tab'],
          },
          {
            path: '/',
            component: '@/components/login/guide',
            routes: [
              //需要登录后可见的放到这里
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
