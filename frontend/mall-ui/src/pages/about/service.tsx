import { AutoAwesome, Grade, Redeem, ThumbUp } from '@mui/icons-material';
import { Box, Grid, Typography } from '@mui/material';
import XBigMenu from './components/box';

export default function IndexPage() {
  return (
    <>
      <Box sx={{ my: 3, px: 1, display: 'none' }}>
        <Typography variant="h3" gutterBottom sx={{}}>
          我们的服务
        </Typography>
        <Grid
          container
          spacing={{
            xs: 1,
            sm: 2,
            md: 3,
          }}
        >
          <Grid item xs={6} sm={6} md={6}>
            <XBigMenu
              title="礼品方案"
              desc=""
              color="warning"
              //path={`/collection`}
              icon={<Redeem />}
            />
          </Grid>
          <Grid item xs={6} sm={6} md={6}>
            <XBigMenu
              title="平替计划"
              desc=""
              color="error"
              //path={`/replace`}
              icon={<AutoAwesome />}
            />
          </Grid>
          <Grid item xs={6} sm={6} md={6}>
            <XBigMenu
              title="VIP大会员"
              desc=""
              color="info"
              //path={`/about/vip`}
              icon={<Grade />}
            />
          </Grid>
          <Grid item xs={6} sm={6} md={6}>
            <XBigMenu
              title="美妆教程"
              desc=""
              color="secondary"
              //path={`/pages`}
              icon={<ThumbUp />}
            />
          </Grid>
        </Grid>
      </Box>
    </>
  );
}
