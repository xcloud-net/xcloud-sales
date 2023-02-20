import XImage from '@/components/image';
import u from '@/utils';
import { AfterSaleDto, AfterSalesItemDto } from '@/utils/models';
import { Alert, Box, Grid, Typography } from '@mui/material';
import { GoodsDto } from '@/utils/models';

export default function AlignItemsList(props: { model: AfterSaleDto }) {
  const { model } = props;

  const renderOrderItem = (item: AfterSalesItemDto) => {
    const { OrderItem } = item;
    if (OrderItem == null || OrderItem == undefined) {
      return <Alert>商品不存在</Alert>;
    }
    const { Goods, GoodsSpecCombination } = OrderItem;
    const pic = u.first(Goods?.XPictures || []);
    var url = u.resolveUrlv2(pic, {
      width: 300,
      height: 300,
    }) as string;

    return (
      <>
        <Box
          sx={{
            p: 1,
            '&:hover': {
              border: '1px solid #ccc',
              borderColor: (theme) => theme.palette.primary.main,
            },
          }}
        >
          <Grid container spacing={2}>
            <Grid item xs={4}>
              <XImage
                src={url}
                height={100}
                width={100}
                fit="cover"
                style={{
                  borderRadius: 4,
                }}
                placeholder
              />
            </Grid>
            <Grid item xs={8}>
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'row',
                  alignItems: 'center',
                  justifyContent: 'flex-end',
                }}
              >
                <Box sx={{}}>
                  <Typography variant="body1">
                    {Goods?.Name}/{GoodsSpecCombination?.Name}
                  </Typography>
                  <Typography
                    variant="overline"
                    color="primary"
                    sx={{ display: 'inline' }}
                  >
                    {`x${item.Quantity}`}
                  </Typography>
                </Box>
              </Box>
            </Grid>
          </Grid>
        </Box>
      </>
    );
  };

  return (
    <Box sx={{}}>
      {u.isEmpty(model.Items) || (
        <Box sx={{}}>
          {u.map(model.Items, (x, index) => (
            <Box key={index}>{renderOrderItem(x)}</Box>
          ))}
        </Box>
      )}
    </Box>
  );
}
