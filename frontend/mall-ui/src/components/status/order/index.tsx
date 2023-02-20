import { OrderDto } from '@/utils/models';
import utils from '@/utils/order';
import { Typography, Stack, Divider } from '@mui/material';
import XAfterSaleStatus from './aftersale';
import XDeliveryStatus from './delivery';
import XPaymentStatus from './payment';

export default function AlignItemsList(props: { model: OrderDto }) {
  const { model } = props;

  const { IsAftersales } = model;

  const renderStatus = () => {
    if (IsAftersales) {
      return (
        <>
          <div
            style={{
              display: 'inline-block',
            }}
          >
            <Stack
              direction="row"
              divider={<Divider orientation="vertical" flexItem />}
              spacing={1}
            >
              <Typography
                variant="overline"
                sx={{
                  display: 'inline',
                  color: (theme) => theme.palette.error.main,
                }}
              >
                售后中💁‍♂️
              </Typography>
              <XAfterSaleStatus model={model.AfterSales || {}} />
            </Stack>
          </div>
        </>
      );
    }

    if (model.OrderStatusId == utils.OrderStatus.Pending) {
      return <XPaymentStatus model={model} />;
    }

    if (model.OrderStatusId == utils.OrderStatus.Processing) {
      return (
        <Typography
          variant="overline"
          sx={{ display: 'inline', color: (theme) => theme.palette.info.main }}
        >
          等候处理⌛️
        </Typography>
      );
    }

    if (model.OrderStatusId == utils.OrderStatus.Cancelled) {
      return (
        <Typography
          variant="overline"
          sx={{ display: 'inline', color: (theme) => theme.palette.error.main }}
        >
          已取消❌
        </Typography>
      );
    }

    if (model.OrderStatusId == utils.OrderStatus.Delivering) {
      return (
        <>
          <div
            style={{
              display: 'inline-block',
            }}
          >
            <Stack
              direction="row"
              divider={<Divider orientation="vertical" flexItem />}
              spacing={1}
            >
              <Typography
                variant="overline"
                sx={{
                  display: 'inline',
                  color: (theme) => theme.palette.secondary.main,
                }}
              >
                配送中🚗
              </Typography>
              <XDeliveryStatus model={model} />
            </Stack>
          </div>
        </>
      );
    }

    if (model.OrderStatusId == utils.OrderStatus.Complete) {
      return (
        <Typography
          variant="overline"
          sx={{
            display: 'inline',
            color: (theme) => theme.palette.success.main,
          }}
        >
          完成✅
        </Typography>
      );
    }

    const orderStatus = utils.getOrderStatus(model.OrderStatusId);
    if (orderStatus) {
      return (
        <Typography
          variant="overline"
          sx={{
            display: 'inline',
            color: (theme) => theme.palette.primary.main,
          }}
        >
          {orderStatus.name}
        </Typography>
      );
    }

    return '未知状态';
  };

  return <>{renderStatus()}</>;
}
