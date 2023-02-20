import u from '@/utils';
import { Input, Modal } from 'antd';

import * as React from 'react';

export default function Animations(props: any) {
  const { model, ok, show, hide } = props;

  const [loadingSave, _loadingSave] = React.useState(false);

  const [comment, _comment] = React.useState('');

  const save = () => {
    _loadingSave(true);
    u.http.apiRequest
      .post('/mall-admin/aftersale/cancel', {
        Id: model.Id,
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
        title="确认取消售后订单吗？"
        open={show}
        confirmLoading={loadingSave}
        okText="确认取消"
        onOk={() => {
          save();
        }}
        onCancel={() => {
          hide && hide();
        }}
      >
        <Input.TextArea
          placeholder="取消理由"
          value={comment}
          onChange={(e) => _comment(e.target.value)}
        />
      </Modal>
    </>
  );
}
