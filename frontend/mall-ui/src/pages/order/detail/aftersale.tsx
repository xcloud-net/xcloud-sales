import u from '@/utils';
import { OrderDto } from '@/utils/models';
import { Box, Typography } from '@mui/material';
import XAfterSales from '../aftersale/detail';

export default function ComplexGrid({ model }: { model: OrderDto }) {
  if (model.IsAftersales && !u.isEmpty(model.AfterSalesId)) {
  } else {
    return <></>;
  }
  return (
    <>
      <Box
        sx={{
          mx: 1,
          mb: 1,
        }}
      >
        <Typography variant="h6" component={'div'} gutterBottom>
          售后中
        </Typography>
        <Box
          sx={{
            p: 1,
            backgroundColor: 'rgb(250,250,250)',
            borderRadius: 1,
            border: '1px dashed gray',
            borderColor: (theme) => theme.palette.primary.main,
          }}
        >
          <XAfterSales detailId={model.AfterSalesId} />
        </Box>
      </Box>
    </>
  );
}
