import * as React from 'react';

import XEmpty from '@/components/empty';
import XLoading from '@/components/loading/dots';
import u from '@/utils';
import http from '@/utils/http';
import { Box, Checkbox, Container, FormControlLabel, Paper, Stack, Typography } from '@mui/material';
import { useModel } from 'umi';
import XCheckout from './checkout';
import XItem from './item';
import { RadioButtonUncheckedOutlined } from '@mui/icons-material';
import TaskAltOutlinedIcon from '@mui/icons-material/TaskAltOutlined';
import { useLocalStorageState } from 'ahooks';
import { ShoppingCartItemDto } from '@/utils/models';

const index = (props: any) => {
  const shoppingcartModel = useModel('storeAppShoppingcart');
  const appSettings = useModel('storeAppSetting');

  const [loading, _loading] = React.useState(false);
  const [data, _data] = React.useState<ShoppingCartItemDto[]>([]);
  const [selected, _selected] = React.useState<number[]>([]);

  const selectedItems = u.filter(data, (x) => selected.indexOf(x.Id || 0) >= 0);

  const [selectedCache, _selectedCache] = useLocalStorageState<string>(
    'shoppingcart.selected.cached',
  );

  const triggerUpdateShoppingcartCount = () => {
    shoppingcartModel.queryShoppingcartCount();
  };

  const queryShoppingcarts = () => {
    _loading(true);
    http.apiRequest
      .post('/mall/shoppingcart/list')
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data.Data || []);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  React.useEffect(() => {
    //_selected(u.map(data, (x) => x.Id));
  }, [data]);

  React.useEffect(() => {
    //triggerUpdateShoppingcartCount();
    queryShoppingcarts();

    try {
      if (!u.isEmpty(selectedCache)) {
        _selected(JSON.parse(selectedCache || '[]'));
      }
    } catch (e) {
      console.log(e);
    }
  }, []);

  React.useEffect(() => {
    _selectedCache(JSON.stringify(selected));
  }, [selected]);

  React.useEffect(() => {
    var originColor = document.body.style.backgroundColor;
    document.body.style.backgroundColor = 'rgb(251, 251, 251)';
    return () => {
      document.body.style.backgroundColor = originColor;
    };
  }, []);

  const renderSelectAll = () => {
    if (u.isEmpty(data)) {
      return null;
    }

    var selectAll = u.every(data, (x) => selected.indexOf(x.Id || 0) >= 0);

    return (
      <FormControlLabel
        control={
          <Checkbox
            checked={selectAll}
            onChange={(e) => {
              _selected(e.target.checked ? u.map(data, (x) => x.Id || 0) : []);
            }}
            icon={<RadioButtonUncheckedOutlined />}
            checkedIcon={<TaskAltOutlinedIcon color='success' />}
          />
        }
        label={'全选'}
      />
    );
  };

  const renderTotalBox = () => {
    if (u.isEmpty(selectedItems)) {
      return renderSelectAll();
    }

    var originPrice = u.sumBy(
      selectedItems,
      (x) => (x.GoodsSpecCombination?.Price || 0) * (x.Quantity || 0),
    );

    var finalPrice = u.sumBy(
      selectedItems,
      (x) =>
        (x.GoodsSpecCombination?.GradePrice ||
          x.GoodsSpecCombination?.Price ||
          0) * (x.Quantity || 0),
    );

    var offset = originPrice - finalPrice;

    return (
      <Box>
        <strong>{selectedItems.length}</strong>个商品，总计
        <strong>{finalPrice}</strong>元。
        {offset > 0 && (
          <span>
            优惠<strong>{offset}</strong>元
          </span>
        )}
      </Box>
    );
  };

  if (loading) {
    return <XLoading />;
  }

  if (u.isEmpty(data)) {
    return <XEmpty />;
  }

  return (
    <>
      <Container maxWidth='sm' disableGutters>
        <Box sx={{ px: 1, py: 2 }}>
          <Box
            sx={{
              mb: 3,
            }}
          >
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                alignItems: 'center',
                justifyContent: 'space-between',
                py: 3,
                px: 1,
              }}
            >
              <Stack
                direction={'row'}
                alignItems='flex-end'
                justifyContent={'flex-start'}
                spacing={1}
              >
                <Typography variant='h2'>购物车</Typography>
                {data.length > 0 && (
                  <Typography variant='h3'>({data.length})</Typography>
                )}
              </Stack>
              <Box sx={{}}>{renderSelectAll()}</Box>
            </Box>
            {u.map(data || [], (x, index) => (
              <Box key={index}>
                <XItem
                  model={x}
                  checked={selected.indexOf(x.Id || 0) >= 0}
                  onUpdate={() => {
                    queryShoppingcarts();
                  }}
                  onDelete={() => {
                    triggerUpdateShoppingcartCount();
                    queryShoppingcarts();
                  }}
                  onSelect={(id: number, checked: boolean) => {
                    console.log(id, checked);
                    if (checked) {
                      _selected([...selected, id]);
                    } else {
                      _selected(u.without(selected, id));
                    }
                  }}
                />
              </Box>
            ))}
          </Box>
          <Box
            sx={{
              position: 'fixed',
              left: 0,
              right: 0,
              bottom: `${appSettings.bottomHeight + 5}px`,
              zIndex: 10,
            }}
          >
            <Container disableGutters maxWidth='sm'>
              <Paper
                sx={{
                  px: 1,
                  py: 1,
                  mx: 1,
                  border: (theme) => `2px solid ${theme.palette.error.main}`,
                  display: 'flex',
                  flexDirection: 'row',
                  alignItems: 'center',
                  justifyContent: 'space-between',
                }}
                elevation={2}
              >
                <Box>{renderTotalBox()}</Box>
                <Box>
                  <XCheckout selectedItems={selectedItems} />
                </Box>
              </Paper>
            </Container>
          </Box>
        </Box>
      </Container>
    </>
  );
};

export default index;
