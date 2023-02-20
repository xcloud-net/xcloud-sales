import u from '@/utils';
import { OrderDto } from '@/utils/models';
import { Alert, Box } from '@mui/material';
import Paper from '@mui/material/Paper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableRow from '@mui/material/TableRow';
import XDeliveryStatus from '@/components/status/order/delivery';

export default function BasicTable(props: { model: OrderDto }) {
  const { model } = props;

  if (!model.ShippingRequired) {
    return (
      <Box sx={{}}>
        <Alert>此商品无需配送</Alert>
      </Box>
    );
  }

  return (
    <TableContainer component={Paper}>
      <Table sx={{}} size="small">
        <TableBody>
          <TableRow sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
            <TableCell component="th" scope="row">
              配送状态
            </TableCell>
            <TableCell align="right">
              <XDeliveryStatus model={model} />
            </TableCell>
          </TableRow>
          <TableRow sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
            <TableCell component="th" scope="row">
              发货时间
            </TableCell>
            <TableCell align="right">
              {u.dateTimeFromNow(model.ShippingTime || '') || '--'}
            </TableCell>
          </TableRow>
          <TableRow sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
            <TableCell component="th" scope="row">
              收货人
            </TableCell>
            <TableCell align="right">
              {model.ShippingAddressContactName || '--'}
            </TableCell>
          </TableRow>
          <TableRow sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
            <TableCell component="th" scope="row">
              联系电话
            </TableCell>
            <TableCell align="right">
              {model.ShippingAddressContact || '--'}
            </TableCell>
          </TableRow>
          <TableRow sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
            <TableCell component="th" scope="row">
              配送地址
            </TableCell>
            <TableCell align="right">
              {model.ShippingAddressDetail || '--'}
            </TableCell>
          </TableRow>
        </TableBody>
      </Table>
    </TableContainer>
  );
}
