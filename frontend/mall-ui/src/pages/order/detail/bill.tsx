import u from '@/utils';
import utils from '@/utils/order';
import { Chip, TableHead } from '@mui/material';
import Paper from '@mui/material/Paper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableRow from '@mui/material/TableRow';
import { useEffect, useState } from 'react';

export default function BasicTable(props: any) {
  const { model } = props;
  const [bills, _bills] = useState<any>([]);
  const [loading, _loading] = useState(false);

  const queryBill = () => {
    if (u.isEmpty(model.Id)) {
      return;
    }
    _loading(true);
    u.http.apiRequest
      .post('/mall/order/list-order-bill', {
        Id: model.Id,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          _bills(res.data.Data || []);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryBill();
  }, [model]);

  return (
    <>
      {loading && <span>loading...</span>}
      {u.isEmpty(bills) && null}
      {u.isEmpty(bills) || (
        <TableContainer component={Paper}>
          <Table sx={{}} size="small">
            <TableHead>
              <TableRow>
                <TableCell>支付渠道</TableCell>
                <TableCell align="right">金额</TableCell>
                <TableCell align="right">支付时间</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {u.map(bills, (x, index) => {
                const {
                  PaymentMethod,
                  Price,
                  PayTime,
                  CreationTime,
                  Refunded,
                } = x;
                const channel = u.find(
                  utils.allStatus.paymentMethod,
                  (d) => d.id == PaymentMethod,
                );

                return (
                  <TableRow
                    sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                  >
                    <TableCell component="th" scope="row">
                      {channel?.name || '未知渠道'}
                    </TableCell>
                    <TableCell align="right">
                      <span>{Price}</span>
                      {Refunded && <Chip color="error" label="已退款"></Chip>}
                    </TableCell>
                    <TableCell align="right">
                      {u.dateTimeFromNow(PayTime || CreationTime)}
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </>
  );
}
