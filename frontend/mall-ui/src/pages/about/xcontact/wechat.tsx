import DouyaQr from '@/assets/douya-qr.png';
import u from '@/utils';
import MailOutlineIcon from '@mui/icons-material/MailOutline';
import { Box, Button, Typography } from '@mui/material';
import { Image } from 'antd-mobile';

export default function Types() {
  return (
    <Box sx={{ py: 3, px: 1, backgroundColor: 'white' }}>
      <Typography variant="button" component={'div'} gutterBottom>
        方式一：
      </Typography>
      <Typography
        variant="overline"
        color="text.disabled"
        component={'div'}
        gutterBottom
      >
        添加微信并保持联系
      </Typography>
      <Box sx={{}}>
        <Image src={DouyaQr} alt="" width="auto" height="auto" />
      </Box>
      <Typography
        variant="overline"
        color="primary"
        component={'div'}
        gutterBottom
      >
        请长按二维码加【豆芽】微信
      </Typography>
      <Box sx={{ py: 2 }}>
        <Button
          variant="outlined"
          fullWidth
          onClick={() => {
            navigator.clipboard
              .writeText(`13921293392`)
              .then(() => {
                u.success('复制成功');
              })
              .catch(() => {
                u.error('复制失败');
              });
          }}
          startIcon={<MailOutlineIcon />}
        >
          微信号：13921293392，点击复制
        </Button>
      </Box>
    </Box>
  );
}
