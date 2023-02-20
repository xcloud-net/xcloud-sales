import u from '@/utils';
import { LoadingButton } from '@mui/lab';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';

import * as React from 'react';

export default function Animations(props: any) {
  const { model, ok, show, hide } = props;

  const [loadingSave, _loadingSave] = React.useState(false);

  const save = () => {
    _loadingSave(true);
    u.http.apiRequest
      .post('/mall/order/complete', {
        OrderId: model.Id,
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

  return (
    <>
      <Dialog open={show} onClose={() => hide && hide()} fullWidth>
        <DialogTitle>确认收货</DialogTitle>
        <DialogContent>
          <DialogContentText>确认收货将完成订单流程！</DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => hide && hide()}>取消</Button>
          <LoadingButton
            onClick={() => {
              save();
            }}
            loading={loadingSave}
          >
            确认收货
          </LoadingButton>
        </DialogActions>
      </Dialog>
    </>
  );
}
