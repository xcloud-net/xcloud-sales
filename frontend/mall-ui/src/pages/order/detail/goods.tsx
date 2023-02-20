import u from '@/utils';
import { Box, Typography, Divider } from '@mui/material';
import XOrderItemList from '../components/orderItemList';

export default function ComplexGrid(props: any) {
  const { model } = props;

  const renderFooter = () => {
    return (
      <Box sx={{ pt: 1, display: 'flex', justifyContent: 'space-between' }}>
        <Typography
          variant="overline"
          color="primary"
          sx={{
            color: 'gray',
            display: 'inline',
          }}
        >
          {u.dateTimeFromNow(model.CreationTime)}
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
    <>
      <XOrderItemList model={model} />
      <Divider />
      {renderFooter()}
    </>
  );
}
