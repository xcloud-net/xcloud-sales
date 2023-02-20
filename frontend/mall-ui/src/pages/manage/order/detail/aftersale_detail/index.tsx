import u from '@/utils';
import { Col, Row, Spin } from 'antd';
import { useEffect, useState } from 'react';
import XAction from './action';
import XGoods from './goods';
import XSummary from './summary';

export default (props: { detailId: string }) => {
  const { detailId } = props;
  const [loading, _loading] = useState(false);
  const [data, _data] = useState<any>([]);

  const queryOrder = () => {
    if (u.isEmpty(detailId)) {
      return;
    }
    _loading(true);
    u.http.apiRequest
      .post('/mall-admin/aftersale/by-id', { Id: detailId })
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
          <Col span={24}>
            <XSummary model={data} />
            <XAction
              model={data}
              ok={() => {
                queryOrder();
              }}
            />
            <XGoods model={data} />
          </Col>
          <Col span={8}></Col>
        </Row>
      </Spin>
    </>
  );
};
