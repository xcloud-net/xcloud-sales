import utils from '@/utils/order';
import {
  CheckOutlined,
  Close,
  PaymentOutlined,
  Warning,
} from '@mui/icons-material';
import { Button, ButtonGroup } from '@mui/material';
import { useState } from 'react';
import XCancel from './cancel';
import XDelivery from './delivery';
import XPayment from './payment';
import XStatus from './status';

export default (props: any) => {
  const { model, ok } = props;

  const [showCancel, _showCancel] = useState(false);
  const [showPayment, _showPayment] = useState(false);
  const [showDelivery, _showDelivery] = useState(false);
  const [showStatus, _showStatus] = useState(false);

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
      <XPayment
        model={model}
        show={showPayment}
        hide={() => _showPayment(false)}
        ok={() => {
          triggerReload();
          _showPayment(false);
        }}
      />
      <XStatus
        model={model}
        show={showStatus}
        hide={() => _showStatus(false)}
        ok={() => {
          triggerReload();
          _showStatus(false);
        }}
      />
      <div
        style={{
          display: 'flex',
          flexDirection: 'row',
          alignItems: 'center',
          justifyContent: 'center',
          marginBottom: 40,
          marginTop: 30,
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
              线下支付
            </Button>
          </ButtonGroup>
        )}
        {status == utils.OrderStatus.Processing && (
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
              startIcon={<CheckOutlined />}
              onClick={() => {
                _showDelivery(true);
              }}
            >
              发货
            </Button>
          </ButtonGroup>
        )}
        {status == utils.OrderStatus.Complete && (
          <ButtonGroup size="large" fullWidth></ButtonGroup>
        )}
        <ButtonGroup size="large">
          <Button
            startIcon={<Warning />}
            color="error"
            onClick={() => {
              _showStatus(true);
            }}
          >
            强制修改状态
          </Button>
        </ButtonGroup>
      </div>
    </>
  );
};
