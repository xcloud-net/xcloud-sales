import u from '@/utils';
import { LoadingButton } from '@mui/lab';
import { Box, Checkbox, Divider, TextField, Typography } from '@mui/material';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import { Stepper } from 'antd-mobile';

import * as React from 'react';

export default function Animations(props: any) {
  const { model, ok, show, hide } = props;
  const items = model.Items || [];

  const forminitial = {
    ReasonForReturn: '',
    RequestedAction: '',
    UserComments: 'empty for now',
    StaffNotes: 'empty for now',
  };

  const [aftersaleForm, _aftersaleForm] = React.useState(forminitial);
  const [selectedItems, _selectedItems] = React.useState<any>([]);
  const [loadingSave, _loadingSave] = React.useState(false);

  const save = () => {
    if (u.isEmpty(selectedItems)) {
      return;
    }
    if (
      u.isEmpty(aftersaleForm.ReasonForReturn) ||
      u.isEmpty(aftersaleForm.RequestedAction)
    ) {
      u.error('请输入售后诉求和原因');
      return;
    }

    _loadingSave(true);
    u.http.apiRequest
      .post('/mall/aftersale/create', {
        OrderId: model.Id,
        Items: selectedItems,
        ...aftersaleForm,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          ok && ok();
        });
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  React.useEffect(() => {
    if (show) {
      _selectedItems(
        u.map(items, (x) => ({ OrderItemId: x.Id, Quantity: x.Quantity })),
      );
      _aftersaleForm(forminitial);
    }
  }, [show]);

  const buildItemsRow = (row: any) => {
    const { Goods, GoodsSpecCombination, Price, Quantity, Id } = row;
    var name = `${Goods?.Name}-${GoodsSpecCombination?.Name}`;

    var selectedObj = u.find(selectedItems, (x) => x.OrderItemId == Id);
    var selected = selectedObj != null && selectedObj != undefined;

    return (
      <>
        <Box
          sx={{
            py: 2,
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'space-between',
          }}
        >
          <Typography
            variant="overline"
            color="primary"
            component="div"
            sx={{
              flexGrow: 1,
            }}
          >
            {name}
          </Typography>
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'row',
              alignItems: 'center',
              justifyContent: 'flex-end',
            }}
          >
            <Stepper
              style={{
                marginLeft: 10,
                marginRight: 10,
                width: 100,
              }}
              value={selectedObj?.Quantity || 0}
              min={1}
              max={Quantity}
              disabled={!selected}
              onChange={(e) => {
                if (selected) {
                  var obj = {
                    ...selectedObj,
                    Quantity: e,
                  };
                  _selectedItems([
                    ...u.filter(selectedItems, (x) => x.OrderItemId != Id),
                    obj,
                  ]);
                }
              }}
            />
            <Checkbox
              checked={selected}
              onChange={(e) => {
                var exclued = u.filter(
                  selectedItems,
                  (x) => x.OrderItemId != Id,
                );
                if (e.target.checked) {
                  exclued.push({
                    OrderItemId: Id,
                    Quantity: Quantity,
                  });
                }
                _selectedItems(exclued);
              }}
            />
          </Box>
        </Box>
        <Divider />
      </>
    );
  };

  return (
    <>
      <Dialog
        open={show}
        onClose={() => hide && hide()}
        fullWidth
        scroll="body"
      >
        <DialogTitle>发起售后</DialogTitle>
        <DialogContent>
          <DialogContentText>
            请选择需要售后的商品，订单只能发起一次售后
          </DialogContentText>
          <Box
            sx={{
              mb: 2,
            }}
          >
            {u.isEmpty(items) ||
              u.map(items, (x, index) => (
                <Box key={index}>{buildItemsRow(x)}</Box>
              ))}
          </Box>
          <Box sx={{ mb: 2 }}>
            <TextField
              sx={{ mb: 2, width: '100%' }}
              placeholder={'输入退货或者换货'}
              label={'售后诉求'}
              value={aftersaleForm.RequestedAction}
              onChange={(e) => {
                _aftersaleForm({
                  ...aftersaleForm,
                  RequestedAction: e.target.value,
                });
              }}
            />
            <TextField
              sx={{ mb: 2, width: '100%' }}
              multiline
              minRows={3}
              placeholder={'简单描述一下售后原因'}
              label={'售后原因'}
              value={aftersaleForm.ReasonForReturn}
              onChange={(e) => {
                _aftersaleForm({
                  ...aftersaleForm,
                  ReasonForReturn: e.target.value,
                });
              }}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => hide && hide()}>取消</Button>
          <LoadingButton
            onClick={() => {
              save();
            }}
            disabled={u.isEmpty(selectedItems)}
            loading={loadingSave}
          >
            发起售后
          </LoadingButton>
        </DialogActions>
      </Dialog>
    </>
  );
}
