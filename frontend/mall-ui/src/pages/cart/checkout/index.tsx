import * as React from 'react';

import XAddressSelector from '@/components/addressSelector';
import u from '@/utils';
import http from '@/utils/http';
import { ArrowCircleRightOutlined, CheckOutlined } from '@mui/icons-material';
import { LoadingButton } from '@mui/lab';
import { history, useModel } from 'umi';
import XAddress from './address';
import XCoupon from './coupon';

import {
  Alert,
  Box,
  Button,
  Dialog,
  DialogContent,
  DialogTitle,
  Divider,
  FormControlLabel,
  Stack,
  Switch,
  TextField,
} from '@mui/material';

const index = (props: any) => {
  const shoppingcartModel = useModel('storeAppShoppingcart');
  const appSettings = useModel('storeAppSetting');

  const { selectedItems } = props;

  const [open, _open] = React.useState(false);

  const [formdata, _formdata] = React.useState({
    ShippingRequired: true,
    AddressId: '',
    AddressProvince: '',
    AddressCity: '',
    AddressArea: '',
    AddressDetail: '',
    AddressContact: '',
    AddressPhone: '',
    CouponId: null,
    Remark: '',
  });

  const [placeOrderLoading, _placeOrderLoading] = React.useState(false);

  const triggerUpdateShoppingcartCount = () => {
    shoppingcartModel.queryShoppingcartCount();
  };

  const placeOrder = () => {
    console.log('placeOrder', formdata);
    if (u.isEmpty(selectedItems)) {
      alert('请选择商品');
      return;
    }

    if (u.isEmpty(formdata.AddressId)) {
      u.error('请选择收货地址');
      return;
    }

    if (
      u.isEmpty(formdata.AddressContact) ||
      u.isEmpty(formdata.AddressDetail) ||
      u.isEmpty(formdata.AddressPhone)
    ) {
      u.error('请输入联系人/手机号/收件地址');
      return;
    }

    if (!u.validator.isMobile(formdata.AddressPhone)) {
      u.error('请输入正确的手机号');
      return;
    }

    //goto checkout
    _placeOrderLoading(true);

    var items = u.map(selectedItems, (x) => ({
      GoodsSpecCombinationId: x.GoodsSpecCombinationId,
      Quantity: x.Quantity,
    }));

    http.apiRequest
      .post('/mall/order/place-order', {
        ...formdata,
        Items: items,
      })
      .then((res) => {
        if (res.data.Error) {
          alert(res.data.Error.Message);
        } else {
          u.success('下单成功');

          var order = res.data.Data || {};
          console.log('下单成功', order);
          //goto order detail page
          setTimeout(() => {
            triggerUpdateShoppingcartCount();
            //var detailUrl = `/order/detail?id=${order.Id}`;
            history.push(`/order`);
          }, 1000);
        }
      })
      .finally(() => {
        _placeOrderLoading(false);
      });
  };

  React.useEffect(() => {
    appSettings.queryMallSettings();
  }, []);

  if (appSettings.mallSettings.PlaceOrderDisabled) {
    return <Alert>管理员关闭了下单功能</Alert>;
  }

  return (
    <>
      <Button
        color="warning"
        variant="contained"
        endIcon={<ArrowCircleRightOutlined />}
        disabled={u.isEmpty(selectedItems)}
        onClick={() => {
          _open(true);
        }}
      >
        立即下单
      </Button>
      <Dialog
        fullWidth
        maxWidth="sm"
        scroll="body"
        open={open}
        onClose={() => {
          _open(false);
        }}
      >
        <DialogTitle>购物车下单</DialogTitle>
        <DialogContent>
          <Box
            sx={{
              mb: 3,
              pt: 2,
            }}
          >
            <Stack spacing={3} direction="column">
              {u.isEmpty(appSettings.mallSettings.PlaceOrderNotice) || (
                <Box sx={{}}>
                  <Alert>{appSettings.mallSettings.PlaceOrderNotice}</Alert>
                </Box>
              )}

              {false && <XAddressSelector />}
              <XCoupon />
              <XAddress
                onSelect={(e: string) => {
                  console.log('selected address id:', e);
                }}
                onSelectedAddress={(address: any) => {
                  _formdata({
                    ...formdata,
                    AddressId: address.Id,
                    AddressContact: address.Name,
                    AddressProvince: address.Province,
                    AddressCity: address.City,
                    AddressArea: address.Area,
                    AddressDetail: address.AddressDetail,
                    AddressPhone: address.Tel,
                  });
                }}
              />
              <TextField
                label="收件人"
                value={formdata.AddressContact}
                onChange={(e) => {
                  _formdata({
                    ...formdata,
                    AddressContact: e.target.value,
                  });
                }}
              />
              <TextField
                label="联系电话"
                value={formdata.AddressPhone}
                onChange={(e) => {
                  _formdata({
                    ...formdata,
                    AddressPhone: e.target.value,
                  });
                }}
              />
              <TextField
                multiline
                label="邮寄地址"
                rows={4}
                value={formdata.AddressDetail}
                onChange={(e) => {
                  _formdata({
                    ...formdata,
                    AddressDetail: e.target.value,
                  });
                }}
              />
              <TextField
                multiline
                label="备注"
                rows={2}
                value={formdata.Remark}
                onChange={(e) => {
                  _formdata({
                    ...formdata,
                    Remark: e.target.value,
                  });
                }}
              />
              <FormControlLabel label="延迟发货？" control={<Switch />} />
            </Stack>
          </Box>
          <Divider />
          <Box
            sx={{
              my: 2,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
            }}
          >
            <LoadingButton
              loading={placeOrderLoading}
              variant="contained"
              size="large"
              color="warning"
              startIcon={<CheckOutlined />}
              onClick={() => {
                placeOrder();
              }}
            >
              确认下单
            </LoadingButton>
          </Box>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default index;
