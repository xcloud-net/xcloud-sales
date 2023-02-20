import { Box, Grid } from '@mui/material';
import XCount from './count';
import XTopbrands from './topbrands';
import XTopCategory from './topcategory';
import XTopSeller from './topseller';
import XTopSku from './topsku';
import XTopuser from './topuser';
import XTopVisitedGoods from './topVisitedGoods';

export default () => {
  return (
    <>
      <Box sx={{}}>
        <Box sx={{ mb: 2 }}>
          <XCount />
        </Box>
        <Box sx={{ mb: 2 }}>
          <Grid container spacing={2} sx={{ mb: 2 }}>
            <Grid item xs={6}>
              <XTopVisitedGoods />
            </Grid>
            <Grid item xs={6}>
              <XTopuser />
              <XTopSeller />
              <XTopCategory />
              <XTopbrands />
              <XTopSku />
            </Grid>
          </Grid>
        </Box>
      </Box>
    </>
  );
};
