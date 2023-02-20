import u from '@/utils';
import { OrderDto } from '@/utils/models';
import {
  Alert,
  Box,
  Card,
  CardActionArea,
  Divider,
  Typography,
} from '@mui/material';
import XAftersaleStatus from '@/components/status/order/aftersale';
import XOrderItemList from '../components/orderItemList';
import XStatus from '@/components/status/order';

export default function AlignItemsList(props: { model: OrderDto }) {
  const { model } = props;

  const { IsAftersales } = model;

  const tryRenderAfterSaleTips = () => {
    if (!IsAftersales) {
      return null;
    }
    if (model.AfterSales == null) {
      return <Alert severity="warning">售后数据未关联</Alert>;
    }
    return (
      <>
        <Box
          sx={{
            mb: 1,
            p: 1,
            borderRadius: 1,
            border: '1px dashed gray',
            borderColor: (theme) => theme.palette.primary.main,
            backgroundColor: 'rgb(250,250,250)',
          }}
        >
          <Typography variant="overline" color="text.disabled">
            售后状态：
          </Typography>
          <XAftersaleStatus model={model.AfterSales} />
        </Box>
      </>
    );
  };

  const renderFooter = () => {
    return (
      <Box
        sx={{
          pt: 1,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
        }}
      >
        <Typography
          variant="overline"
          color="primary"
          sx={{
            color: 'gray',
            display: 'inline',
          }}
        >
          {u.dateTimeFromNow(model.CreationTime || '') || '--'}
        </Typography>

        <Typography
          variant="overline"
          color="primary"
          sx={{ display: 'inline' }}
        >
          {`总计：${model.OrderTotal}元`}
        </Typography>
      </Box>
    );
  };

  return (
    <Card>
      <CardActionArea sx={{ p: 2 }}>
        <Box sx={{ mb: 1, display: 'flex', justifyContent: 'space-between' }}>
          <Typography variant="h6" sx={{ display: 'inline' }}>
            #{model.OrderSn}
          </Typography>
          <Box sx={{}}>
            <XStatus model={model} />
          </Box>
        </Box>
        {tryRenderAfterSaleTips()}
        {u.isEmpty(model.Items) || (
          <Box sx={{}}>
            <XOrderItemList model={model} />
          </Box>
        )}
        <Divider />
        {renderFooter()}
      </CardActionArea>
    </Card>
  );
}
