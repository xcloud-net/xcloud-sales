import {
  BankOutlined,
  BarChartOutlined,
  BarsOutlined,
  BuildOutlined,
  CarryOutOutlined,
  ClockCircleOutlined,
  CloseCircleOutlined,
  CodeOutlined,
  ContainerOutlined,
  CreditCardOutlined,
  CrownOutlined,
  DashboardOutlined,
  DotChartOutlined,
  EnvironmentOutlined,
  FlagOutlined,
  FolderOpenOutlined,
  FolderViewOutlined,
  GiftOutlined,
  GoldOutlined,
  GroupOutlined,
  HomeOutlined,
  NotificationOutlined,
  PayCircleOutlined,
  PieChartOutlined,
  PlusOutlined,
  ProjectOutlined,
  SearchOutlined,
  SettingOutlined,
  ShopFilled,
  ShoppingCartOutlined,
  TagOutlined,
  UserOutlined
} from '@ant-design/icons';
import { MenuDataItem } from '@ant-design/pro-layout';
  
  type LayoutMenu = Omit<MenuDataItem, 'routes'> & {
    routes?: MenuDataItem[];
  };
  
  const routes: Array<LayoutMenu> = [
    {
      name: '统计',
      path: '#',
      key: 'dashboard',
      icon: <BarChartOutlined />,
      routes: [
        {
          name: '统计报表',
          path: '/manage/dashboard/home',
          icon: <DashboardOutlined />,
        },
        {
          name: '销售统计',
          path: '/manage/dashboard/sales',
          icon: <DotChartOutlined />,
        },
        {
          name: '搜索关键词',
          path: '/manage/dashboard/keywords',
          icon: <SearchOutlined />,
        },
        {
          name: '用户活跃时间',
          path: '/manage/dashboard/activity',
          icon: <ClockCircleOutlined />,
        },
        {
          name: '活跃城市',
          path: '/manage/dashboard/activity-geo',
          icon: <EnvironmentOutlined />,
        },
      ],
    },
    {
      name: '商品',
      path: '#',
      key: 'catalog',
      icon: <ShoppingCartOutlined />,
      routes: [
        {
          name: '商品管理',
          path: '/manage/goods/list',
          icon: <FolderOpenOutlined />,
        },
        {
          name: '规格明细',
          path: '/manage/goods/combination',
          icon: <FolderViewOutlined />,
        },
        {
          name: '品牌管理',
          path: '/manage/brand/list',
          icon: <FlagOutlined />,
        },
        {
          name: '分类管理',
          path: '/manage/category/list',
          icon: <BarsOutlined />,
        },
        {
          name: '标签管理',
          path: '/manage/tag/list',
          icon: <TagOutlined />,
        },
      ],
    },
    {
      name: '订单',
      path: '#',
      key: 'order',
      icon: <CreditCardOutlined />,
      routes: [
        {
          name: '订单列表',
          path: '/manage/order/list',
          icon: <CarryOutOutlined />,
        },
        {
          name: '售后列表',
          path: '/manage/aftersales/list',
          icon: <CloseCircleOutlined />,
        },
      ],
    },
    {
      name: '营销',
      path: '#',
      key: 'marketing',
      icon: <PieChartOutlined />,
      routes: [
        {
          name: '商品集',
          path: '/manage/marketing/collection',
          icon: <GroupOutlined />,
        },
        {
          name: '优惠券',
          path: '/manage/marketing/coupon',
          icon: <GiftOutlined />,
        },
        {
          name: '促销活动',
          path: '/manage/marketing/promotion',
          icon: <NotificationOutlined />,
        },
      ],
    },
    {
      name: '库存',
      path: '#',
      key: 'warehouse',
      icon: <ProjectOutlined />,
      routes: [
        {
          name: '采购单',
          path: '/manage/stock/stock',
          icon: <PlusOutlined />,
        },
        {
          name: '库存查询',
          path: '/manage/stock/stock-items',
          icon: <GoldOutlined />,
        },
        {
          name: '仓库管理',
          path: '/manage/stock/warehouse',
          icon: <HomeOutlined />,
        },
        {
          name: '供应商管理',
          path: '/manage/stock/supplier',
          icon: <UserOutlined />,
        },
      ],
    },
    {
      name: '财务',
      path: '#',
      key: 'finance',
      icon: <BankOutlined />,
      routes: [
        {
          name: '支付查询',
          path: '/manage/finance/order-bill',
          icon: <PayCircleOutlined />,
        },
        {
          name: '预售卡',
          path: '/manage/prepaidcard/list',
          icon: <GiftOutlined />,
        },
      ],
    },
    {
      name: '系统',
      path: '#',
      key: 'settings',
      icon: <SettingOutlined />,
      routes: [
        {
          name: '商城会员',
          path: '/manage/users',
          icon: <UserOutlined />,
        },
        {
          name: '门店管理',
          path: '/manage/store/list',
          icon: <ShopFilled />,
        },
        {
          name: '会员等级',
          path: '/manage/user-grade',
          icon: <CrownOutlined />,
        },
        {
          name: '首页设计',
          path: '/manage/settings/home-design',
          icon: <BuildOutlined />,
        },
        {
          name: '商城设置',
          path: '/manage/settings/mall',
          icon: <SettingOutlined />,
        },
        {
          name: '环境变量',
          path: '/manage/settings/env',
          icon: <CodeOutlined />,
        },
        {
          name: '平台用户',
          path: '/manage/settings/user/list',
          icon: <UserOutlined />,
        },
        {
          name: '系统管理员',
          path: '/manage/settings/administrator',
          icon: <UserOutlined />,
        },
        {
          name: '省市区',
          path: '/manage/settings/region',
          icon: <EnvironmentOutlined />,
        },
        {
          name: '活动日志',
          path: '/manage/settings/log',
          icon: <ContainerOutlined />,
        },
      ],
    },
  ];
  
  export default routes;
  