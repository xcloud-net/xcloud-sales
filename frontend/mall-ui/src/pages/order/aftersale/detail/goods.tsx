import u from '@/utils';
import { AfterSaleDto, AfterSalesItemDto } from '@/utils/models';
import { Box, Typography } from '@mui/material';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableRow from '@mui/material/TableRow';

export default (props: { model: AfterSaleDto }) => {
  const { model } = props;

  const renderGoods = (item: AfterSalesItemDto) => {
    const { OrderItem } = item;
    const { Goods, GoodsSpecCombination } = OrderItem || {};

    return (
      <>
        <Typography variant="body1">
          {Goods?.Name}/{GoodsSpecCombination?.Name}
        </Typography>
      </>
    );
  };

  return (
    <Box sx={{ my: 1 }}>
      <Typography variant="subtitle1" gutterBottom>
        售后商品
      </Typography>
      <Table sx={{ width: '100%', my: 1 }} size="small">
        <TableBody>
          {u.map(model.Items || [], (x, index) => {
            return (
              <TableRow key={index} hover>
                <TableCell>{renderGoods(x)}</TableCell>
                <TableCell>
                  <Typography
                    variant="overline"
                    color="primary"
                    sx={{ display: 'inline' }}
                  >
                    {`x${x.Quantity}`}
                  </Typography>
                </TableCell>
              </TableRow>
            );
          })}
        </TableBody>
      </Table>
    </Box>
  );
};
