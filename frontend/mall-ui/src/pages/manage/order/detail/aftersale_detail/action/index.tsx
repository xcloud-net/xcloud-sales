import utils from '@/utils/order';
import {
  CheckOutlined,
  Close,
  PaymentOutlined,
  Warning,
} from '@mui/icons-material';
import { Button, ButtonGroup, Stack } from '@mui/material';
import { useState } from 'react';
import XApprove from './approve';
import XCancel from './cancel';
import XComplete from './complete';
import XReject from './reject';
import XStatus from './status';

export default (props: any) => {
  const { model, ok } = props;

  const [showCancel, _showCancel] = useState(false);
  const [showApprove, _showApprove] = useState(false);
  const [showReject, _showReject] = useState(false);
  const [showComplete, _showComplete] = useState(false);
  const [showStatus, _showStatus] = useState(false);

  const status = model.AfterSalesStatusId;

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
      <XComplete
        model={model}
        show={showComplete}
        hide={() => _showComplete(false)}
        ok={() => {
          triggerReload();
          _showComplete(false);
        }}
      />
      <XApprove
        model={model}
        show={showApprove}
        hide={() => _showApprove(false)}
        ok={() => {
          triggerReload();
          _showApprove(false);
        }}
      />
      <XReject
        model={model}
        show={showReject}
        hide={() => _showReject(false)}
        ok={() => {
          triggerReload();
          _showReject(false);
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
          marginBottom: 40,
          marginTop: 30,
        }}
      >
        <Stack
          spacing={2}
          direction="row"
          alignItems={'center'}
          justifyContent="center"
        >
          {status == utils.AftersalesStatus.Procesing && (
            <ButtonGroup size="large">
              <Button
                startIcon={<Close />}
                color="error"
                onClick={() => {
                  _showReject(true);
                }}
              >
                拒绝
              </Button>
              <Button
                startIcon={<PaymentOutlined />}
                onClick={() => {
                  _showApprove(true);
                }}
              >
                批准
              </Button>
            </ButtonGroup>
          )}
          <ButtonGroup size="large">
            {status == utils.AftersalesStatus.Approved && (
              <Button
                startIcon={<CheckOutlined />}
                color="success"
                onClick={() => {
                  _showComplete(true);
                }}
              >
                完结售后请求
              </Button>
            )}
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
        </Stack>
      </div>
    </>
  );
};
