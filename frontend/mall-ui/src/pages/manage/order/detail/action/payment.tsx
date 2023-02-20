import u from '@/utils';
import { Typography } from '@mui/material';
import { message, Modal } from 'antd';
import * as React from 'react';

export default function Animations(props: any) {
  const { model, ok, show, hide } = props;

  const [loadingSave, _loadingSave] = React.useState(false);

  const startManulPay = async () => {
    if (u.isEmpty(model.Id)) {
      return;
    }
    _loadingSave(true);
    try {
      var billResponse = await u.http.apiRequest.post(
        '/mall-admin/order/create-order-bill',
        { Id: model.Id },
      );
      if (!u.handleResponse(billResponse, () => {})) {
        throw billResponse;
      }
      var bill = billResponse.data.Data || {};
      var res = await u.http.apiRequest.post(
        '/mall-admin/order/mark-bill-as-paid',
        { Id: bill.Id },
      );
      u.handleResponse(res, () => {
        message.success('支付成功');
        hide && hide();
        ok && ok();
      });
    } catch (e) {
      console.log('manul pay', e);
    } finally {
      _loadingSave(false);
    }
  };

  return (
    <>
      <Modal
        title="收银台"
        open={show}
        onCancel={() => {
          hide && hide();
        }}
        onOk={() => {
          startManulPay();
        }}
        okText="标记订单为已支付"
        confirmLoading={loadingSave}
      >
        <Typography variant="h6" component={'div'} gutterBottom>
          当买家已经在线下完成支付，可以在此手动将订单标记为【已支付】
        </Typography>
      </Modal>
    </>
  );
}
