import u from '@/utils';
import { OrderDto } from '@/utils/models';
import { Tooltip } from '@mui/material';
import Paper from '@mui/material/Paper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableRow from '@mui/material/TableRow';
import XPaymentStatus from '@/components/status/order/payment';
import XStatus from '@/components/status/order';

export default function BasicTable(props: { model: OrderDto }) {
  const { model } = props;

  return (
    <TableContainer component={Paper}>
      <Table sx={{}} size="small">
        <TableBody>
          <TableRow sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
            <TableCell component="th" scope="row">
              订单号
            </TableCell>
            <TableCell align="right">{model.OrderSn}</TableCell>
          </TableRow>
          <TableRow sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
            <TableCell component="th" scope="row">
              订单状态
            </TableCell>
            <TableCell align="right">
              <XStatus model={model} />
            </TableCell>
          </TableRow>
          <TableRow sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
            <TableCell component="th" scope="row">
              创建时间
            </TableCell>
            <TableCell align="right">
              <Tooltip title={model.CreationTime || '--'}>
                <span>
                  {u.dateTimeFromNow(model.CreationTime || '') || '--'}
                </span>
              </Tooltip>
            </TableCell>
          </TableRow>
          <TableRow sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
            <TableCell component="th" scope="row">
              支付状态
            </TableCell>
            <TableCell align="right">
              <XPaymentStatus model={model} />
            </TableCell>
          </TableRow>
          <TableRow sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
            <TableCell component="th" scope="row">
              支付时间
            </TableCell>
            <TableCell align="right">
              {u.dateTimeFromNow(model.PaidTime || '') || '--'}
            </TableCell>
          </TableRow>
        </TableBody>
      </Table>
    </TableContainer>
  );
}
