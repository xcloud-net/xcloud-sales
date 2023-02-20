import u from '@/utils';
import { Spin, Row, Col } from 'antd';
import { useEffect, useState } from 'react';
import XSummary from './summary';
import XGoods from './goods';
import XAction from './action';
import XBill from './bill';
import XNotes from './notes';
import XShipping from './shipping';
import XAftersale from './aftersale';
import { OrderDto } from '@/utils/models';

export default (props: { detailId: string }) => {
  const { detailId } = props;
  const [loading, _loading] = useState(false);
  const [data, _data] = useState<OrderDto>({});

  const queryOrder = () => {
    if (u.isEmpty(detailId)) {
      return;
    }
    _loading(true);
    u.http.apiRequest
      .post('/mall-admin/order/detail', { Id: detailId })
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data.Data || {});
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryOrder();
  }, [detailId]);

  return (
    <>
      <Spin spinning={loading}>
        <Row gutter={10}>
          <Col span={16}>
            {data.IsAftersales && <XAftersale model={data} />}
            <XSummary model={data} />
            {data.IsAftersales || (
              <XAction
                model={data}
                ok={() => {
                  queryOrder();
                }}
              />
            )}
            <XGoods model={data} />
            <XShipping model={data} />
            <XBill model={data} />
          </Col>
          <Col span={8}>
            <XNotes model={data} />
          </Col>
        </Row>
      </Spin>
    </>
  );
};
