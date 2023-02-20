import XGuide from '@/assets/guide.png';
import XWebQr from '@/assets/web-qr.jpeg';
import u from '@/utils';
import { Image } from 'antd-mobile';
import { Box, Dialog, Grid, Typography } from '@mui/material';
import { useState } from 'react';

export default () => {
  const [previewUrl, _previewUrl] = useState('');

  return (
    <>
      <Dialog
        open={!u.isEmpty(previewUrl)}
        scroll="body"
        fullWidth
        onClose={() => {
          _previewUrl('');
        }}
      >
        <Image
          src={previewUrl}
          alt=""
          style={{
            width: '100%',
            height: '100%',
          }}
        />
      </Dialog>
      <Box sx={{ mb: 3, px: 1, py: 1, backgroundColor: 'white' }}>
        <Typography variant="button" component={'div'} gutterBottom>
          方式二：
        </Typography>
        <Typography
          variant="overline"
          color="text.disabled"
          component={'div'}
          gutterBottom
        >
          请收藏我们的站点以保持长期联系
        </Typography>
        <Grid container spacing={2}>
          <Grid item xs={6} sm={6} md={4}>
            <Image
              onClick={() => {
                _previewUrl(XWebQr);
              }}
              src={XWebQr}
              alt=""
            />
            <Typography variant="overline" color="primary">
              长按下载或者转发
            </Typography>
          </Grid>
          <Grid item xs={6} sm={6} md={4}>
            <Image
              onClick={() => {
                _previewUrl(XGuide);
              }}
              src={XGuide}
              alt=""
            />
            <Typography variant="overline" color="primary">
              将站点加入微信浮动窗口
            </Typography>
          </Grid>
        </Grid>
      </Box>
    </>
  );
};
