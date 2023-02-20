import { OrderDto } from '@/utils/models';
import utils from '@/utils/order';
import { Typography } from '@mui/material';

export default function AlignItemsList(props: { model: OrderDto }) {
  const { model } = props;

  const renderStatus = () => {
    if (model.PaymentStatusId == utils.PaymentStatus.Paid) {
      return (
        <Typography
          variant="overline"
          sx={{
            display: 'inline',
            color: (theme) => theme.palette.success.main,
          }}
        >
          已支付💰
        </Typography>
      );
    }

    if (model.PaymentStatusId == utils.PaymentStatus.Pending) {
      return (
        <Typography
          variant="overline"
          sx={{
            display: 'inline',
            color: (theme) => theme.palette.primary.main,
          }}
        >
          待支付⌛️
        </Typography>
      );
    }

    const status = utils.allStatus.paymentStatus.find(
      (x) => x.status == model.PaymentStatusId,
    );

    return (
      <Typography
        variant="overline"
        sx={{ display: 'inline', color: (theme) => theme.palette.primary.main }}
      >
        {status?.name || '--'}
      </Typography>
    );
  };

  return <>{renderStatus()}</>;
}
