import { AfterSaleDto } from '@/utils/models';
import { Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField } from '@mui/material';
import { useEffect, useState } from 'react';
import u from '@/utils';

export default ({ model, ok }: { model: AfterSaleDto, ok?: any }) => {
  const [show, _show] = useState(false);
  const [content, _content] = useState('');
  const [loading, _loading] = useState(false);

  const sendComment = () => {
    if (u.isEmpty(content)) {
      u.error('请输入内容');
      return;
    }
    _loading(true);
    u.http.apiRequest.post('/mall/aftersale/add-comment', {
      AfterSaleId: model.Id,
      Content: content,
      Pictures: null,
    }).then(res => {
      u.handleResponse(res, () => {
        u.success('发布成功');
        _show(false);
        ok && ok();
      });
    }).finally(() => {
      _loading(false);
    });
  };

  useEffect(() => {
    show && _content('');
  }, [show]);

  return <>
    <Dialog open={show} fullWidth onClose={() => {
      _show(false);
    }}>
      <DialogTitle>发表内容</DialogTitle>
      <DialogContent>
        <TextField sx={{
          width: '100%',
        }} variant={'outlined'} placeholder={'请输入...'} multiline rows={3} value={content} onChange={e => {
          _content(e.target.value);
        }} />
      </DialogContent>
      <DialogActions>
        <Button variant={'text'} onClick={() => {
          sendComment();
        }} disabled={loading}>{loading ? `发送中...` : `发送`}</Button>
      </DialogActions>
    </Dialog>
    <Box sx={{
      my: 1,
      backgroundColor: `rgb(250, 250, 250)`,
    }}>
      <TextField sx={{
        width: '100%',
      }} size={'small'} variant={'outlined'} placeholder={'请输入...'} onFocus={e => {
        e.preventDefault();
        e.target.blur();
      }} onClick={(e) => {
        e.preventDefault();
        _show(true);
      }} />
    </Box>
  </>;
};
