const ActivityLogTypes = {
  Login: -1,
  VisiteGoods: 1,
  SearchGoods: 2,
  PlaceOrder: 3,
  CreatePayment: 4,
  VisitePage: 5,
  GetCoupon: 6,
  AddShoppingCart: 7,
  DeleteShoppingCart: 8,
  AddFavorite: 9,
  DeleteFavorite: 10,
  AuditLog: 11,
};

const ActivityLogTypesDescription = [
  {
    id: ActivityLogTypes.Login,
    name: '登录',
  },
  {
    id: ActivityLogTypes.VisiteGoods,
    name: '查看商品',
  },
  {
    id: ActivityLogTypes.SearchGoods,
    name: '搜索商品',
  },
  {
    id: ActivityLogTypes.PlaceOrder,
    name: '下单',
  },
  {
    id: ActivityLogTypes.CreatePayment,
    name: '唤起支付',
  },
  {
    id: ActivityLogTypes.VisitePage,
    name: '查看页面',
  },
  {
    id: ActivityLogTypes.GetCoupon,
    name: '领取优惠券',
  },
  {
    id: ActivityLogTypes.AddShoppingCart,
    name: '添加购物车',
  },
  {
    id: ActivityLogTypes.DeleteShoppingCart,
    name: '删除购物车',
  },
  {
    id: ActivityLogTypes.AddFavorite,
    name: '添加收藏',
  },
  {
    id: ActivityLogTypes.DeleteFavorite,
    name: '删除收藏',
  },
  {
    id: ActivityLogTypes.AuditLog,
    name: '审计日志',
  },
];

export default {
  ActivityLogTypes,
  ActivityLogTypesDescription,
};
