import u from '@/utils';

const OrderStatus = {
  None: 0,
  Pending: 10,
  Processing: 20,
  Delivering: 23,
  Complete: 30,
  Cancelled: 40,
  AfterSale: 50,
};

const PaymentStatus = {
  Pending: 0,
  Voided: 1,
  PartiallyPaid: 2,
  Paid: 3,
  PartiallyRefunded: 4,
  Refunded: 5,
};

const PaymentMethod = {
  None: 0,
  Manual: 1,
  Balance: 2,
  WechatMp: 3,
  WechatOpen: 4,
};

const ShippingStatus = {
  ShippingNotRequired: 10,
  NotYetShipped: 20,
  PartiallyShipped: 25,
  Shipped: 30,
  Delivered: 40,
};

const AftersalesStatus = {
  None: 0,
  Procesing: 1,
  Approved: 2,
  Rejected: 3,
  Complete: 4,
  Cancelled: 5,
};

const allStatus = {
  ShippingStatus: [
    {
      status: ShippingStatus.ShippingNotRequired,
      name: '无需配送',
    },
    {
      status: ShippingStatus.NotYetShipped,
      name: '未处理',
    },
    {
      status: ShippingStatus.PartiallyShipped,
      name: '部分装车发出',
    },
    {
      status: ShippingStatus.Shipped,
      name: '装车发出',
    },
    {
      status: ShippingStatus.Delivered,
      name: '物流配送中',
    },
  ],
  AftersalesStatus: [
    {
      status: AftersalesStatus.None,
      name: '初始状态',
    },
    {
      status: AftersalesStatus.Procesing,
      name: '处理中',
    },
    {
      status: AftersalesStatus.Approved,
      name: '已批准',
    },
    {
      status: AftersalesStatus.Rejected,
      name: '被拒绝',
      color: 'warning',
    },
    {
      status: AftersalesStatus.Complete,
      name: '完成',
      color: 'success',
    },
    {
      status: AftersalesStatus.Cancelled,
      name: '取消',
      color: 'error',
    },
  ],
  orderStatus: [
    {
      status: OrderStatus.Pending,
      name: '进行中',
    },
    {
      status: OrderStatus.Processing,
      name: '处理中',
    },
    {
      status: OrderStatus.Delivering,
      name: '配送中',
    },
    {
      status: OrderStatus.Complete,
      name: '已完成',
      color: 'success',
    },
    {
      status: OrderStatus.Cancelled,
      name: '已取消',
      color: 'error',
    },
    {
      status: OrderStatus.AfterSale,
      name: '完成售后',
      color: 'error',
    },
  ],
  paymentMethod: [
    {
      id: PaymentMethod.Manual,
      name: '线下支付',
    },
    {
      id: PaymentMethod.Balance,
      name: '余额支付',
    },
    {
      id: PaymentMethod.WechatMp,
      name: '微信公众号支付',
    },
    {
      id: PaymentMethod.WechatOpen,
      name: '微信小程序支付',
    },
  ],
  paymentStatus: [
    {
      status: PaymentStatus.Pending,
      name: '未支付',
    },
    {
      status: PaymentStatus.Voided,
      name: '无需支付',
    },
    {
      status: PaymentStatus.PartiallyPaid,
      name: '部分支付',
    },
    {
      status: PaymentStatus.Paid,
      name: '已支付',
      color: 'success',
    },
    {
      status: PaymentStatus.PartiallyRefunded,
      name: '部分退款',
    },
    {
      status: PaymentStatus.Refunded,
      name: '已退款',
    },
  ],
};

const getOrderStatus = (status: any) => {
  const item = u.find(allStatus.orderStatus, (x) => x.status == status);

  return item;
};

const getPaymentMethod = (method: any) => {
  const item = u.find(allStatus.paymentMethod, (x) => x.id == method);

  return item;
};

const getPaymentStatus = (status: any) => {
  const item = u.find(allStatus.paymentStatus, (x) => x.status == status);

  return item;
};

export default {
  OrderStatus,
  PaymentStatus,
  PaymentMethod,
  ShippingStatus,
  AftersalesStatus,
  allStatus,
  getOrderStatus,
  getPaymentMethod,
  getPaymentStatus,
};
