import u from '@/utils';
import { WechatOutlined } from '@ant-design/icons';
import {
  Badge,
  Box,
  Button,
  Checkbox,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControlLabel,
  Stack,
  Typography,
} from '@mui/material';
import qs from 'qs';
import { useEffect, useState } from 'react';

export default function (props: any) {
  const [openWechat, _openWechat] = useState(false);
  const [usewechatprofile, _usewechatprofile] = useState(true);
  const usewechatprofileKey = 'usewechatprofile';
  useEffect(() => {
    _usewechatprofile(localStorage.getItem(usewechatprofileKey) == 'true');
  }, []);

  useEffect(() => {
    localStorage.setItem(
      usewechatprofileKey,
      usewechatprofile ? 'true' : 'false',
    );
  }, [usewechatprofile]);

  function isWechat() {
    return /MicroMessenger/i.test(window.navigator.userAgent);
  }

  const wechatLogin = isWechat();

  const startWxMpAuthorize = () => {
    var callback = u.concatUrl([
      window.location.origin,
      '/store/account/wx-callback',
    ]);
    console.log(callback);
    //callback = encodeURIComponent(callback);
    //var next = encodeURIComponent(``);
    var p = qs.stringify(
      {
        appid: u.config.wx.mp.appid,
        redirect_uri: callback,
        response_type: 'code',
        scope: 'snsapi_userinfo',
        state: 'STATE',
      },
      {
        addQueryPrefix: false,
      },
    );

    var url = 'https://open.weixin.qq.com/connect/oauth2/authorize';
    url = `${url}?${p}#wechat_redirect`;
    console.log(url);
    window.location.href = url;
  };

  return (
    <>
      <Dialog
        open={openWechat}
        onClose={() => {
          _openWechat(false);
        }}
      >
        <DialogTitle>使用微信授权登录</DialogTitle>
        <DialogContent>
          <Typography variant="overline" color={'primary'} component="div">
            即将跳转到微信授权登录！
          </Typography>
          <FormControlLabel
            control={
              <Checkbox
                checked={usewechatprofile}
                onChange={(e) => {
                  _usewechatprofile(e.target.checked);
                }}
              />
            }
            label="使用微信的昵称和头像"
          />
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => {
              _openWechat(false);
            }}
          >
            取消
          </Button>
          <Button
            onClick={() => {
              startWxMpAuthorize();
            }}
          >
            确定
          </Button>
        </DialogActions>
      </Dialog>
      <Box sx={{ py: 3 }}>
        <Stack
          spacing={2}
          direction={'row'}
          alignContent="center"
          justifyContent={'flex-end'}
        >
          {wechatLogin && (
            <Badge badgeContent="推荐" color="error">
              <Button
                onClick={() => {
                  _openWechat(true);
                }}
                variant="outlined"
                startIcon={<WechatOutlined />}
                color="success"
              >
                微信登录
              </Button>
            </Badge>
          )}
        </Stack>
      </Box>
    </>
  );
}
