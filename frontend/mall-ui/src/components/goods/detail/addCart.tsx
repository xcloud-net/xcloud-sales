import XCombinationPrice from '@/components/goods/price';
import XLoginCheck from '@/components/login/check';
import u from '@/utils';
import http from '@/utils/http';
import logger from '@/utils/logger';
import { GoodsCombinationDto } from '@/utils/models';
import { AddOutlined } from '@mui/icons-material';
import { LoadingButton } from '@mui/lab';
import { Box, Divider, Grid, Typography } from '@mui/material';
import { Stepper } from 'antd-mobile';
import * as React from 'react';
import { history } from 'umi';
import XPriceHistory from './priceHistory';

export default function CustomDeleteIconChips(props: {
  selectedCombination?: GoodsCombinationDto;
}) {
  const { selectedCombination } = props;

  const [addCartCount, _addCartCount] = React.useState(1);
  const [addCartLoading, _addCartLoading] = React.useState(false);

  const combinationValid =
    selectedCombination && selectedCombination.Id && selectedCombination.Id > 0;

  React.useEffect(() => {
    _addCartCount(1);
  }, [selectedCombination]);

  const addToCart = () => {
    if (!combinationValid) {
      return;
    }

    _addCartLoading(true);

    http.apiRequest
      .post('/mall/shoppingcart/add-v1', {
        GoodsSpecCombinationId: selectedCombination.Id,
        Quantity: addCartCount,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          logger.log({
            Comment: '添加购物车',
          });

          u.success('添加成功');

          setTimeout(() => {
            history.push('/shoppingcart');
          }, 500);
        });
      })
      .finally(() => {
        _addCartLoading(false);
      });
  };

  return (
    <>
      {combinationValid && (
        <>
          <Divider />
          <Box sx={{ padding: 2 }}>
            <Grid container spacing={1}>
              <Grid item xs={12} sm={12} md={6}>
                <Box
                  sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    alignItems: 'center',
                    justifyContent: {
                      xs: 'space-between',
                      sm: 'space-between',
                      md: 'flex-start',
                    },
                  }}
                >
                  <Typography variant="h6" sx={{ mr: 3 }}>
                    {selectedCombination?.Name || '--'}
                  </Typography>
                  <Typography variant="subtitle1">
                    <XCombinationPrice model={selectedCombination || {}} />
                  </Typography>
                  <XLoginCheck>
                    <Box sx={{ ml: 1 }}>
                      <XPriceHistory id={selectedCombination?.Id} />
                    </Box>
                  </XLoginCheck>
                </Box>
              </Grid>
              <Grid item xs={12} sm={12} md={6}>
                <Box
                  sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    alignItems: 'center',
                    justifyContent: {
                      xs: 'space-between',
                      sm: 'space-between',
                      md: 'flex-end',
                    },
                  }}
                >
                  <Stepper
                    value={addCartCount}
                    onChange={(e) => {
                      _addCartCount(e || 1);
                    }}
                    style={{ marginRight: 15 }}
                    min={1}
                    max={Math.min(
                      100,
                      selectedCombination?.StockQuantity || 100,
                    )}
                  ></Stepper>
                  <XLoginCheck>
                    <LoadingButton
                      variant="contained"
                      color="primary"
                      loading={addCartLoading}
                      startIcon={<AddOutlined />}
                      onClick={() => {
                        addToCart();
                      }}
                    >
                      加入购物车
                    </LoadingButton>
                  </XLoginCheck>
                </Box>
              </Grid>
            </Grid>
          </Box>
        </>
      )}
    </>
  );
}
