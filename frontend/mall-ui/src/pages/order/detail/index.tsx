import u from '@/utils';
import { OrderDto } from '@/utils/models';
import { Alert, Box, Divider, Typography } from '@mui/material';
import { useEffect, useState } from 'react';
import { history } from 'umi';
import XAction from './action';
import XAftersales from './aftersale';
import XBill from './bill';
import XGoods from './goods';
import XShipping from './shipping';
import XStatus from './status';
import XSummary from './summary';
import LinearProgress from '@/components/loading/linear';

export default function ComplexGrid({
  orderId,
  ok,
}: {
  orderId: string;
  ok?: any;
}) {
  const [id, _id] = useState('');
  const [loading, _loading] = useState(false);
  const [data, _data] = useState<OrderDto>({});

  const { IsAftersales } = data;

  const queryOrder = () => {
    if (u.isEmpty(id)) {
      return;
    }
    console.log('utils:', u);
    _loading(true);
    u.http.apiRequest
      .post('/mall/order/detail', {
        Id: id,
      })
      .then((res) => {
        if (res.data.Error) {
          alert(res.data.Error.Message);
        } else {
          _data(res.data.Data || {});
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryOrder();
  }, [id]);

  useEffect(() => {
    _id(orderId);
  }, [orderId]);

  useEffect(() => {
    var queryId = history.location.query?.id as string;
    if (!u.isEmpty(queryId)) {
      _id(queryId);
    }
  }, []);

  if (!loading && u.isEmpty(data.Id)) {
    return <Alert>订单不存在</Alert>;
  }

  return (
    <>
      {loading && <LinearProgress />}
      {loading || (
        <>
          <Box
            sx={{
              my: 2,
              py: 2,
            }}
          >
            <Box sx={{ mb: 3 }}>
              {IsAftersales || <XStatus model={data} />}
              {IsAftersales && <XAftersales model={data} />}
            </Box>
            <Divider />
          </Box>
          <Box sx={{ my: 2 }}>
            {IsAftersales || (
              <XAction
                model={data}
                ok={() => {
                  queryOrder();
                  ok && ok();
                }}
              />
            )}
          </Box>
          <Box sx={{ p: 2, my: 1 }}>
            <Typography variant="h6" gutterBottom>
              订单详情
            </Typography>
            <XSummary model={data} />
          </Box>
          <Box sx={{ p: 2, my: 1 }}>
            <Typography variant="h6" gutterBottom>
              配送信息
            </Typography>
            <XShipping model={data} />
          </Box>
          <Box sx={{ p: 2, my: 1 }}>
            <Typography variant="h6" gutterBottom>
              商品信息
            </Typography>
            <XGoods model={data} />
          </Box>
          <Box sx={{ p: 2, my: 1 }}>
            <Typography variant="h6" gutterBottom>
              账单信息
            </Typography>
            <XBill model={data} />
          </Box>
        </>
      )}
    </>
  );
}
