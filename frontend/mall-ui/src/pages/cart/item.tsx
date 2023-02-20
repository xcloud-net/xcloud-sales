import XDefaultImage from '@/assets/empty.svg';
import XPriceRow from '@/components/goods/combinationPriceRow';
import XImage from '@/components/image';
import LinearProgress from '@/components/loading/linear';
import u from '@/utils';
import http from '@/utils/http';
import { RadioButtonUncheckedOutlined } from '@mui/icons-material';
import TaskAltOutlinedIcon from '@mui/icons-material/TaskAltOutlined';
import WarningAmberIcon from '@mui/icons-material/WarningAmber';
import {
  Box,
  Checkbox,
  Chip,
  FormControlLabel,
  Paper,
  Stack,
} from '@mui/material';
import { Stepper, SwipeAction } from 'antd-mobile';
import { useState } from 'react';
import { GoodsDto } from '@/utils/models';

export default function MiddleDividers(props: any) {
  const { model, onSelect, checked, onDelete, onUpdate } = props;

  const [loading, _loading] = useState(false);

  const goods: GoodsDto = model.Goods;
  const pic = u.first(goods?.XPictures || []);

  const goodsImageUrl = u.resolveUrlv2(pic, {
    width: 100,
    height: 100,
  });

  const showWarning = false;

  const deleteCart = () => {
    if (!confirm('确定删除？')) {
      return;
    }
    _loading(true);
    http.apiRequest
      .post('/mall/shoppingcart/delete', { Id: model.Id })
      .then((res) => {
        u.handleResponse(res, () => {
          onDelete && onDelete();
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  const updateQuantity = (quantity: number) => {
    _loading(true);
    http.apiRequest
      .post('/mall/shoppingcart/update', {
        Id: model.Id,
        Quantity: quantity,
      })
      .then((res) => {
        if (res.data.Error) {
          alert(res.data.Error.Message);
        } else {
          onUpdate && onUpdate();
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const renderWarnings = () => {
    if (u.isEmpty(model.Waring)) {
      return null;
    }
    return (
      <>
        <Stack direction="row" spacing={1}>
          {u.map(model.Waring, (x, index) => (
            <Chip
              key={index}
              size="small"
              color="error"
              icon={<WarningAmberIcon />}
              label={x}
              variant="outlined"
            />
          ))}
        </Stack>
      </>
    );
  };

  return (
    <SwipeAction
      rightActions={[
        {
          color: 'danger',
          text: '删除',
          key: 1,
          onClick: (e) => {
            deleteCart();
          },
        },
      ]}
    >
      <Paper
        sx={{
          mb: 2,
          p: 2,
        }}
        elevation={0}
      >
        {loading && <LinearProgress />}
        <Stack
          direction={'row'}
          alignItems="center"
          justifyContent={'space-between'}
        >
          <FormControlLabel
            control={
              <Checkbox
                checked={checked}
                onChange={(e) => {
                  onSelect && onSelect(model.Id, e.target.checked);
                }}
                icon={<RadioButtonUncheckedOutlined />}
                checkedIcon={<TaskAltOutlinedIcon color="success" />}
              />
            }
            label={`${model.Goods?.Name}`}
          />
        </Stack>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'flex-start',
          }}
        >
          <Box sx={{ width: '70px' }}>
            <XImage alt="" src={goodsImageUrl || XDefaultImage} />
          </Box>
          <Box sx={{ ml: 1, width: '100%' }}>
            <Stack
              direction={'row'}
              alignItems="center"
              justifyContent="space-between"
              spacing={1}
            >
              <Box flexGrow={1} sx={{ mr: 1 }}>
                <XPriceRow model={model.GoodsSpecCombination || {}} />
              </Box>
              <Stepper
                value={model.Quantity}
                min={0}
                max={100}
                onChange={(value) => {
                  if (value <= 0) {
                    if (confirm('向左滑动即可删除！')) {
                      deleteCart();
                    }
                  } else {
                    updateQuantity(value);
                  }
                }}
                style={{
                  '--button-text-color': 'black',
                }}
              />
            </Stack>
          </Box>
        </Box>
        {showWarning && renderWarnings()}
      </Paper>
    </SwipeAction>
  );
}
