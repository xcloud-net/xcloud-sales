import utils from '@/utils/order';
import {
  CheckOutlined,
  Close,
  HomeRepairServiceOutlined,
  PaymentOutlined,
} from '@mui/icons-material';
import { Box, Button, ButtonGroup } from '@mui/material';
import { useState } from 'react';
import XAftersale from './aftersale';
import XCancel from './cancel';
import XDelivery from './delivery';
import XPayment from './payment';

export default function BasicTable(props: any) {
  const { model, ok } = props;

  const [showCancel, _showCancel] = useState(false);
  const [showAftersale, _showAftersale] = useState(false);
  const [showPayment, _showPayment] = useState(false);
  const [showDelivery, _showDelivery] = useState(false);

  const status = model.OrderStatusId;

  const triggerReload = () => ok && ok();

  return (
    <>
      <XCancel
        model={model}
        show={showCancel}
        hide={() => _showCancel(false)}
        ok={() => {
          triggerReload();
          _showCancel(false);
        }}
      />
      <XDelivery
        model={model}
        show={showDelivery}
        hide={() => _showDelivery(false)}
        ok={() => {
          triggerReload();
          _showDelivery(false);
        }}
      />
      <XAftersale
        model={model}
        show={showAftersale}
        hide={() => _showAftersale(false)}
        ok={() => {
          triggerReload(), _showAftersale(false);
        }}
      />
      <XPayment
        model={model}
        show={showPayment}
        hide={() => _showPayment(false)}
        ok={() => {
          triggerReload(), _showPayment(false);
        }}
      />
      <Box
        sx={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          px: 3,
        }}
      >
        {status == utils.OrderStatus.Pending && (
          <ButtonGroup size="large" fullWidth>
            <Button
              startIcon={<Close />}
              color="error"
              onClick={() => {
                _showCancel(true);
              }}
            >
              取消订单
            </Button>
            <Button
              startIcon={<PaymentOutlined />}
              onClick={() => {
                _showPayment(true);
              }}
            >
              支付
            </Button>
          </ButtonGroup>
        )}
        {status == utils.OrderStatus.Delivering && (
          <ButtonGroup size="large" fullWidth>
            <Button
              startIcon={<HomeRepairServiceOutlined />}
              onClick={() => {
                _showAftersale(true);
              }}
            >
              申请售后
            </Button>
            <Button
              startIcon={<CheckOutlined />}
              onClick={() => {
                _showDelivery(true);
              }}
            >
              确认收货
            </Button>
          </ButtonGroup>
        )}
        {status == utils.OrderStatus.Complete && (
          <ButtonGroup size="large" fullWidth></ButtonGroup>
        )}
      </Box>
    </>
  );
}
