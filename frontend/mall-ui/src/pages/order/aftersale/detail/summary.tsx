import u from '@/utils';
import { AfterSaleDto } from '@/utils/models';
import { Box, Typography } from '@mui/material';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableRow from '@mui/material/TableRow';
import XAftersaleStatus from '@/components/status/order/aftersale';

export default (props: { model: AfterSaleDto }) => {
  const { model } = props;

  const { ReasonForReturn, RequestedAction } = model;
  var totalPrice = u.sumBy(
    model.Items || [],
    (x: any) => x.OrderItem?.Price || 0,
  );

  return (
    <Box sx={{ my: 1 }}>
      <Typography variant="subtitle1" gutterBottom>
        售后信息
      </Typography>
      <Table sx={{ width: '100%', my: 1 }} size="small">
        <TableBody>
          <TableRow>
            <TableCell>售后状态</TableCell>
            <TableCell>
              <XAftersaleStatus model={model} />
            </TableCell>
          </TableRow>
          <TableRow>
            <TableCell>发起时间</TableCell>
            <TableCell>
              {u.dateTimeFromNow(model.CreationTime || '') || '--'}
            </TableCell>
          </TableRow>
          <TableRow>
            <TableCell>售后诉求</TableCell>
            <TableCell>{RequestedAction || '--'}</TableCell>
          </TableRow>
          <TableRow>
            <TableCell>售后原因</TableCell>
            <TableCell>{ReasonForReturn || '--'}</TableCell>
          </TableRow>
          <TableRow>
            <TableCell>涉及金额💰</TableCell>
            <TableCell>
              <Typography
                variant="overline"
                color="primary"
                sx={{ display: 'inline' }}
              >
                {`总计：${totalPrice}元`}
              </Typography>
            </TableCell>
          </TableRow>
        </TableBody>
      </Table>
    </Box>
  );
};
