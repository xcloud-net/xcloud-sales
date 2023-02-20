import u from '@/utils';
import utils from '@/utils/order';
import { Alert, Input, Modal, Select, Form } from 'antd';

import * as React from 'react';

export default function Animations(props: any) {
  const { model, ok, show, hide } = props;

  const [loadingSave, _loadingSave] = React.useState(false);

  const [comment, _comment] = React.useState('');
  const [status, _status] = React.useState(-1);

  const { AfterSalesStatusId } = model;

  React.useEffect(() => {
    _status(AfterSalesStatusId);
  }, [AfterSalesStatusId]);

  const save = () => {
    _loadingSave(true);
    u.http.apiRequest
      .post('/mall-admin/aftersale/dangerously-update-status', {
        Id: model.Id,
        Status: status,
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
        title="⚠️强制修改状态"
        open={show}
        confirmLoading={loadingSave}
        okText="危险：强制修改状态"
        onOk={() => {
          save();
        }}
        onCancel={() => {
          hide && hide();
        }}
      >
        <Alert
          style={{ marginBottom: 10 }}
          message="错误的修改状态可能造成数据紊乱，请谨慎操作！"
          type="warning"
        ></Alert>
        <Form>
          <Form.Item>
            <Input.TextArea
              placeholder="取消理由"
              value={comment}
              onChange={(e) => _comment(e.target.value)}
            />
          </Form.Item>
          <Form.Item>
            <Select
              value={status}
              onChange={(e) => {
                _status(e);
              }}
            >
              {u.map(utils.allStatus.AftersalesStatus, (x, index) => (
                <Select.Option key={index} value={x.status}>
                  {x.name}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
}
