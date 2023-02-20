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
      .post('/mall-admin/aftersale/approve', {
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
        title="确认批准售后请求吗？"
        open={show}
        confirmLoading={loadingSave}
        okText="批准售后请求"
        onCancel={() => {
          hide && hide();
        }}
        onOk={() => {
          save();
        }}
      >
        <Input.TextArea
          autoFocus
          placeholder="批准理由"
          value={comment}
          onChange={(e) => _comment(e.target.value)}
        />
      </Modal>
    </>
  );
}
