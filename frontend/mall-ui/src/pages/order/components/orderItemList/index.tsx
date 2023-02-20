import XImage from '@/components/image';
import u from '@/utils';
import { Box, Grid, Typography } from '@mui/material';
import { GoodsDto, GoodsCombinationDto } from '@/utils/models';

export default function AlignItemsList(props: any) {
  const { model } = props;

  const renderOrderItem = (item: any) => {
    const GoodsSpecCombination: GoodsCombinationDto = item.GoodsSpecCombination;
    const Goods: GoodsDto = item.Goods;
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
      {u.map(model.Items || [], (x, index) => (
        <Box key={index}>{renderOrderItem(x)}</Box>
      ))}
    </Box>
  );
}
