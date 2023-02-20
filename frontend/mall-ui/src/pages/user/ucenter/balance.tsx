import XQR from '@/components/qrcode';
import u from '@/utils';
import {
  AccountBalanceWalletRounded,
  CurrencyExchangeRounded,
  QrCode,
} from '@mui/icons-material';
import {
  Box,
  CardActionArea,
  Dialog,
  DialogContent,
  DialogTitle,
  Grid,
  Typography,
} from '@mui/material';
import { useEffect, useState } from 'react';
import { history } from 'umi';

interface IBtn {
  title: string;
  subtitle?: string;
  icon: any;
  action?: any;
  pathname?: string;
}

export default () => {
  const [openQr, _openQr] = useState(false);
  const [data, _data] = useState({
    Balance: 0,
    Points: 0,
  });
  const [loading, _loading] = useState(false);

  const queryData = () => {
    _loading(true);
    u.http.apiRequest
      .post('/mall/user/balance-and-points')
      .then((res) => {
        u.handleResponse(res, () => {
          res.data.Data && _data(res.data.Data);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryData();
  }, []);

  const createButton = (item: IBtn) => {
    return (
      <CardActionArea
        sx={{
          display: 'flex',
          flexDirection: 'row',
          alignItems: 'center',
          justifyContent: 'center',
          py: 2,
          height: '100%',
        }}
        onClick={() => {
          item.action && item.action();
          item.pathname &&
            history.push({
              pathname: item.pathname,
            });
        }}
      >
        <Box sx={{}}>
          {item.icon}
          <Typography variant="subtitle1" component={'div'} gutterBottom>
            {item.title}
          </Typography>
          <Typography
            variant="overline"
            color={'primary'}
            sx={{
              display: 'block',
              textAlign: 'center',
              visibility: u.isEmpty(item.subtitle) ? 'hidden' : 'visible',
            }}
          >
            {item.subtitle || '--'}
          </Typography>
        </Box>
      </CardActionArea>
    );
  };

  return (
    <>
      <Dialog
        open={openQr}
        onClose={() => {
          _openQr(false);
        }}
      >
        <DialogTitle>付款二维码</DialogTitle>
        <DialogContent>
          <XQR value="123" width={128} height={128} />
        </DialogContent>
      </Dialog>

      <Box
        sx={{
          mx: 1,
          mb: 2,
          backgroundColor: 'white',
          //border: loading ? 'none' : '1px solid rgb(0, 127, 255)',
          borderRadius: 1,
          overflow: 'hidden',
          //display: 'none',
        }}
      >
        <Grid container>
          <Grid item xs={4}>
            {createButton({
              icon: <AccountBalanceWalletRounded fontSize="large" />,
              title: '余额',
              subtitle: `${data.Balance}元`,
              pathname: '/ucenter/balance',
            })}
          </Grid>
          <Grid item xs={4}>
            {createButton({
              icon: <QrCode fontSize="large" />,
              title: '付款',
              action: () => {
                _openQr(true);
              },
            })}
          </Grid>
          <Grid item xs={4}>
            {createButton({
              icon: <CurrencyExchangeRounded fontSize="large" />,
              title: '积分',
              subtitle: `${data.Points}`,
              pathname: '/ucenter/points',
            })}
          </Grid>
        </Grid>
      </Box>
    </>
  );
};
