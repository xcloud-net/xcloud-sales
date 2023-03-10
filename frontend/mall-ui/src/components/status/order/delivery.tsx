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
          ιιδΈ­π¦
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
          ε·²θ£θ½¦π
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

    return 'ζͺη₯ηΆζ';
  };

  return <>{renderStatus()}</>;
}
