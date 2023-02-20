import XGoodsCard from '@/components/goods/box/base';
import XDetailPreview from '@/components/goods/detail/detailPreview';
import u from '@/utils';
import { Box, Grid, Typography, Paper } from '@mui/material';
import { useState } from 'react';
import XEmpty from './empty';
import XLoading from './loading';
import { GoodsDto } from '@/utils/models';

export default (props: {
  data: GoodsDto[];
  loading: boolean;
  title: string;
}) => {
  const { data, loading, title } = props;
  const [detailId, _detailId] = useState(0);

  const renderGrid = () => {
    if (u.isEmpty(data)) {
      return <XEmpty />;
    }
    return (
      <Grid
        container
        spacing={{
          xs: 1,
          sm: 1,
          md: 2,
        }}
      >
        {u.map(data || [], (x, index) => (
          <Grid item xs={6} sm={4} md={4} key={index}>
            <Paper
              sx={{
                borderRadius: 1,
                overflow: 'hidden',
                cursor: 'pointer',
                '&:hover': {
                  border: (t) => `1px solid ${t.palette.primary.main}`,
                },
              }}
              elevation={0}
              onClick={() => {
                x.Id && _detailId(x.Id);
              }}
            >
              <XGoodsCard
                data={{
                  model: x,
                  count: 1,
                }}
              />
            </Paper>
          </Grid>
        ))}
      </Grid>
    );
  };

  if (loading) {
    return <XLoading />;
  }

  return (
    <>
      <XDetailPreview
        detailId={detailId}
        onClose={() => {
          _detailId(0);
        }}
      />
      <Box
        sx={{
          px: 1,
          py: 2,
          backgroundColor: (theme) => theme.palette.background.default,
        }}
      >
        {u.isEmpty(title) || (
          <Typography variant="h3" component={'div'} gutterBottom>
            {title}
          </Typography>
        )}
        <Box sx={{}}>{renderGrid()}</Box>
      </Box>
    </>
  );
};
