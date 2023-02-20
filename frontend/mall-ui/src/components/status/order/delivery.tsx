import { OrderDto } from '@/utils/models';
import utils from '@/utils/order';
import { Typography } from '@mui/material';

export default function AlignItemsList(props: { model: OrderDto }) {
  const { model } = props;

  const renderStatus = () => {
    if (model.ShippingStatusId == utils.ShippingStatus.Delivered) {
      return (
        <Typography
          variant="overline"
          sx={{
            display: 'inline',
            color: (theme) => theme.palette.primary.main,
          }}
        >
          配送中📦
        </Typography>
      );
    }

    if (model.ShippingStatusId == utils.ShippingStatus.Shipped) {
      return (
        <Typography
          variant="overline"
          sx={{
            display: 'inline',
            color: (theme) => theme.palette.primary.main,
          }}
        >
          已装车🚗
        </Typography>
      );
    }

    const status = utils.allStatus.ShippingStatus.find(
      (x) => x.status == model.ShippingStatusId,
    );
    if (status) {
      return (
        <Typography
          variant="overline"
          sx={{
            display: 'inline',
            color: (theme) => theme.palette.primary.main,
          }}
        >
          {status.name}
        </Typography>
      );
    }

    return '未知状态';
  };

  return <>{renderStatus()}</>;
}
