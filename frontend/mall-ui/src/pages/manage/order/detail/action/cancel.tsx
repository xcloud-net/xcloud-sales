import u from '@/utils';
import { Typography } from '@mui/material';
import { Input, Modal } from 'antd';

import * as React from 'react';

export default function Animations(props: any) {
  const { model, ok, show, hide } = props;

  const [loadingSave, _loadingSave] = React.useState(false);

  const [comment, _comment] = React.useState('');

  const save = () => {
    _loadingSave(true);
    u.http.apiRequest
      .post('/mall-admin/order/cancel', {
        OrderId: model.Id,
        Comment: comment,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          ok && ok();
        });
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  return (
    <>
      <Modal
        title="取消订单"
        open={show}
        onCancel={() => {
          hide && hide();
        }}
        onOk={() => {
          save();
        }}
        confirmLoading={loadingSave}
      >
        <Typography variant="h6" component={'div'} gutterBottom>
          确认取消订单吗？
        </Typography>
        <Input.TextArea
          autoFocus
          rows={3}
          placeholder="请输入取消理由"
          value={comment}
          onChange={(e) => _comment(e.target.value)}
        />
      </Modal>
    </>
  );
}
