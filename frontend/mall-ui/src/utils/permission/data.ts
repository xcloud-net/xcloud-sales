import { Permission, PermissionGroup } from '@/utils/models';

const data: { groups: PermissionGroup[] } = {
  groups: [
    {
      name: '超级管理员权限',
      permissions: [
        {
          name: '超级管理员权限',
          key: '*',
        },
      ],
    },
    {
      name: '系统设置',
      permissions: [
        {
          name: '商城设置',
          key: 'mall-settings',
        },
        {
          name: '商城用户',
          key: 'mall-user',
        },
        {
          name: '平台用户',
          key: 'platform-user',
        },
        {
          name: '权限设置',
          key: 'permission-settings',
        },
      ],
    },
    {
      name: '报表',
      permissions: [
        {
          name: '全部',
          key: 'dashboard',
        },
      ],
    },
    {
      name: '商品',
      permissions: [
        {
          name: '商品管理',
          key: 'goods-manage',
        },
        {
          name: '品牌管理',
          key: 'brand-manage',
        },
        {
          name: '标签管理',
          key: 'tag-manage',
        },
        {
          name: '分类管理',
          key: 'category-manage',
        },
      ],
    },
    {
      name: '订单和售后',
      permissions: [
        {
          name: '订单管理',
          key: 'order-manage',
        },
        {
          name: '售后管理',
          key: 'aftersale-manage',
        },
      ],
    },
    {
      name: '财务',
      permissions: [
        {
          name: '支付查询',
          key: 'payment-manage',
        },
        {
          name: '退款查询',
          key: 'refund-manage',
        },
        {
          name: '预售卡',
          key: 'prepaid-card-manage',
        },
      ],
    },
    {
      name: '仓库',
      permissions: [
        {
          name: '详细库存',
          key: 'warehouse-stock-quantity',
        },
      ],
    },
  ],
};

export default data;
