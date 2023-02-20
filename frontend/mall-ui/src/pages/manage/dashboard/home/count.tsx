import {
  Grid,
  Paper,
  Typography,
  Box,
  SxProps,
  Theme,
  Tooltip,
} from '@mui/material';
import u from '@/utils';
import { useEffect, useState } from 'react';
import { history } from 'umi';

interface IBox {
  title: string;
  tip?: string;
  count: any;
  action?: any;
  pathname?: string;
  props?: SxProps<Theme>;
}

const xcard = (item: IBox) => {
  const title = (
    <Typography variant="h6" gutterBottom>
      {item.title}
    </Typography>
  );

  return (
    <Paper
      onClick={() => {
        item.action && item.action();
        item.pathname &&
          history.push({
            pathname: item.pathname,
          });
      }}
      sx={{
        borderRadius: 4,
        overflow: 'hidden',
        cursor: 'pointer',
        '&:hover': {
          border: (theme) => `1px solid ${theme.palette.primary.main}`,
        },
      }}
    >
      <Box sx={{ p: 1 }}>
        {u.isEmpty(item.tip) || (
          <Tooltip title={item.tip || '--'}>{title}</Tooltip>
        )}
        {u.isEmpty(item.tip) && title}
      </Box>
      <Box
        sx={{
          p: 1,
          py: 3,
          display: 'flex',
          flexDirection: 'row',
          alignItems: 'center',
          justifyContent: 'center',
          color: 'white',
          ...(item.props || {}),
        }}
      >
        <Typography variant="h3" color={'inherit'}>
          {item.count}
        </Typography>
      </Box>
    </Paper>
  );
};

export default () => {
  const [data, _data] = useState<any>({});

  const goodscount = data.Goods || 0;
  const categorycount = data.Category || 0;
  const brandcount = data.Brand || 0;
  const ordercount = data.Orders || 0;
  const aftersalecount = data.AfterSale || 0;
  const couponcount = data.Coupon || 0;
  const activitycount = data.Promotion || 0;
  const balancesum = data.Balance || 0;
  const prepaidsum = data.PrepaidCard || 0;
  const todayActiveUserCount = data.ActiveUser || 0;

  const queryCounter = () => {
    u.http.apiRequest
      .post('/mall-admin/report/dashboard-counter')
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data.Data || {});
        });
      });
  };

  useEffect(() => {
    queryCounter();
  }, []);

  return (
    <>
      <Box sx={{}}>
        <Typography variant="h4" gutterBottom component={'div'}>
          统计
        </Typography>
        <Grid container spacing={2} sx={{ mb: 2 }}>
          <Grid item xs={2}>
            {xcard({
              title: '今日上线人数',
              count: todayActiveUserCount,
              pathname: '/manage/users',
              props: {
                backgroundColor: (theme) => theme.palette.warning.light,
              },
            })}
          </Grid>
        </Grid>
        <Grid container spacing={2} sx={{ mb: 2 }}>
          <Grid item xs={2}>
            {xcard({
              title: '商品数量',
              count: goodscount,
              pathname: '/manage/goods/list',
              props: {
                backgroundColor: (theme) => theme.palette.primary.light,
              },
            })}
          </Grid>
          <Grid item xs={2}>
            {xcard({
              title: '品牌数量',
              count: brandcount,
              pathname: '/manage/brand/list',
              props: {
                backgroundColor: (theme) => theme.palette.secondary.light,
              },
            })}
          </Grid>
          <Grid item xs={2}>
            {xcard({
              title: '类目数量',
              count: categorycount,
              pathname: '/manage/category/list',
              props: {
                backgroundColor: (theme) => theme.palette.success.light,
              },
            })}
          </Grid>
          <Grid item xs={2}>
            {xcard({
              title: '优惠券数量',
              count: couponcount,
              pathname: '/manage/marketing/coupon',
              props: {
                backgroundColor: (theme) => theme.palette.info.light,
              },
            })}
          </Grid>
          <Grid item xs={2}>
            {xcard({
              title: '店铺活动',
              count: activitycount,
              pathname: '/manage/marketing/activity',
              props: {
                backgroundColor: (theme) => theme.palette.success.light,
              },
            })}
          </Grid>
          <Grid item xs={2}>
            {xcard({
              title: '处理中订单',
              count: ordercount,
              pathname: '/manage/order/list',
              props: {
                backgroundColor: (theme) => theme.palette.warning.light,
              },
            })}
          </Grid>
          <Grid item xs={2}>
            {xcard({
              title: '售后中',
              count: aftersalecount,
              pathname: '/manage/aftersales/list',
              props: {
                backgroundColor: (theme) => theme.palette.error.light,
              },
            })}
          </Grid>
          <Grid item xs={2}>
            {xcard({
              title: '充值卡余额',
              tip: '所有用户账户余额汇总',
              count: balancesum,
              props: {
                backgroundColor: (theme) => theme.palette.success.light,
              },
            })}
          </Grid>
          <Grid item xs={2}>
            {xcard({
              title: '未使用预售卡',
              tip: '所有未使用预售卡金额汇总',
              count: prepaidsum,
              pathname: '/manage/prepaidcard/list',
              props: {
                backgroundColor: (theme) => theme.palette.warning.light,
              },
            })}
          </Grid>
        </Grid>
      </Box>
    </>
  );
};
