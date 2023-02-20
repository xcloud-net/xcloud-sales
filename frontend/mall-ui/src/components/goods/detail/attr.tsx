import XLoading from '@/components/loading';
import u from '@/utils';
import { GoodsDto } from '@/utils/models';
import { Box, Card, CardContent, Typography } from '@mui/material';
import Paper from '@mui/material/Paper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableRow from '@mui/material/TableRow';

export default (props: { model: GoodsDto; loading: boolean }) => {
  const { model, loading } = props;

  const data = model.GoodsAttributes;

  return (
    <>
      <Box sx={{}}>
        {loading && <XLoading />}
        {loading || (
          <>
            {u.isEmpty(data) || (
              <Card sx={{ mt: 1, borderRadius: 0 }}>
                <CardContent>
                  <Typography variant="h6" component="div" gutterBottom>
                    商品属性
                  </Typography>
                  <Box sx={{}}>
                    <TableContainer component={Paper}>
                      <Table sx={{}} size="small">
                        <TableBody>
                          {u.map(data, (x, index) => (
                            <TableRow
                              key={index}
                              sx={{
                                '&:last-child td, &:last-child th': {
                                  border: 0,
                                },
                              }}
                            >
                              <TableCell component="th" scope="row">
                                {x.Name || '--'}
                              </TableCell>
                              <TableCell align="right">
                                {x.Value || '--'}
                              </TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </TableContainer>
                  </Box>
                </CardContent>
              </Card>
            )}
          </>
        )}
      </Box>
    </>
  );
};
