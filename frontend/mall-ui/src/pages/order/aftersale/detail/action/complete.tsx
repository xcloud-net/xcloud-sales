import u from '@/utils';
import { LoadingButton } from '@mui/lab';
import { TextField } from '@mui/material';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import { AfterSaleDto, AfterSalesItemDto } from '@/utils/models';

import * as React from 'react';

export default function Animations(props: {
  model: AfterSaleDto;
  ok: any;
  show: boolean;
  hide: any;
}) {
  const { model, ok, show, hide } = props;

  const [loadingSave, _loadingSave] = React.useState(false);

  const [comment, _comment] = React.useState('');

  const save = () => {
    _loadingSave(true);
    u.http.apiRequest
      .post('/mall/aftersale/complete', {
        Id: model.Id,
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
        <DialogTitle>✅完成售后</DialogTitle>
        <DialogContent>
          <DialogContentText>确认完成售后订单吗？</DialogContentText>
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
            完成售后
          </LoadingButton>
        </DialogActions>
      </Dialog>
    </>
  );
}
