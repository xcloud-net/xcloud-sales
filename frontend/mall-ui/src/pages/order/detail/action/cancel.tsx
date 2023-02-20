import u from '@/utils';
import { LoadingButton } from '@mui/lab';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import { TextField } from '@mui/material';

import * as React from 'react';

export default function Animations(props: any) {
  const { model, ok, show, hide } = props;

  const [loadingSave, _loadingSave] = React.useState(false);

  const [comment, _comment] = React.useState('');

  const save = () => {
    if (u.isEmpty(comment)) {
      u.error('请输入取消理由');
      return;
    }

    _loadingSave(true);
    u.http.apiRequest
      .post('/mall/order/cancel', {
        OrderId: model.Id,
        Comment: comment,
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
        <DialogTitle>取消订单</DialogTitle>
        <DialogContent>
          <DialogContentText>确认取消订单吗？</DialogContentText>
          <TextField
            autoFocus
            label="取消理由"
            multiline
            fullWidth
            value={comment}
            onChange={(e) => _comment(e.target.value)}
            variant="standard"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => hide && hide()}>取消</Button>
          <LoadingButton
            onClick={() => {
              save();
            }}
            loading={loadingSave}
          >
            确认取消
          </LoadingButton>
        </DialogActions>
      </Dialog>
    </>
  );
}
