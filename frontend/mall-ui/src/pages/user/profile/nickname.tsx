import u from '@/utils';
import { LoadingButton } from '@mui/lab';
import { Box, Typography } from '@mui/material';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import TextField from '@mui/material/TextField';
import * as React from 'react';
import XItem from './item';

export default function Animations(props: any) {
  const { model, ok } = props;
  const [open, setOpen] = React.useState(false);

  const [nickname, _nickname] = React.useState('');
  const [loadingSave, _loadingSave] = React.useState(false);

  const handleClickOpen = () => {
    _nickname(model.NickName || '');
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
  };

  const save = () => {
    _loadingSave(true);
    u.http.platformRequest
      .post('/user/update-profile', {
        ...model,
        NickName: nickname,
      })
      .then((res) => {
        var error = res.data.Error;
        if (error) {
          alert(error.Message);
        } else {
          handleClose();
          ok && ok();
        }
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  return (
    <>
      <Dialog open={open} onClose={handleClose}>
        <DialogTitle>修改昵称</DialogTitle>
        <DialogContent>
          <DialogContentText>
            请输入昵称。要求10个字符内，不包含特殊字符和空格。
          </DialogContentText>
          <TextField
            autoFocus
            label="新昵称"
            fullWidth
            value={nickname}
            onChange={(e) => _nickname(e.target.value)}
            variant="standard"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>取消</Button>
          <LoadingButton
            onClick={() => {
              save();
            }}
            loading={loadingSave}
          >
            确认修改
          </LoadingButton>
        </DialogActions>
      </Dialog>
      <Box
        sx={{}}
        onClick={() => {
          handleClickOpen();
        }}
      >
        <XItem
          title={'昵称'}
          right={
            <Typography
              variant="overline"
              sx={{
                color: 'text.secondary',
              }}
            >
              {model.NickName || '--'}
            </Typography>
          }
        />
      </Box>
    </>
  );
}
