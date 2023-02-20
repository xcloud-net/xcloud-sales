import { AfterSaleDto } from '@/utils/models';
import utils from '@/utils/order';
import { Typography } from '@mui/material';

export default function AlignItemsList(props: { model: AfterSaleDto }) {
  const { model } = props;

  const renderStatus = () => {
    if (model.AfterSalesStatusId == utils.AftersalesStatus.Complete) {
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
    if (model.AfterSalesStatusId == utils.AftersalesStatus.Procesing) {
      return (
        <Typography
          variant="overline"
          sx={{
            display: 'inline',
            color: (theme) => theme.palette.info.main,
          }}
        >
          等候处理⌛️
        </Typography>
      );
    }

    if (model.AfterSalesStatusId == utils.AftersalesStatus.Rejected) {
      return (
        <Typography
          variant="overline"
          sx={{
            display: 'inline',
            color: (theme) => theme.palette.error.main,
          }}
        >
          被拒绝❌
        </Typography>
      );
    }

    if (model.AfterSalesStatusId == utils.AftersalesStatus.Approved) {
      return (
        <Typography
          variant="overline"
          sx={{
            display: 'inline',
            color: (theme) => theme.palette.secondary.main,
          }}
        >
          已通过✅
        </Typography>
      );
    }

    const status = utils.allStatus.AftersalesStatus.find(
      (x) => x.status == model.AfterSalesStatusId,
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
