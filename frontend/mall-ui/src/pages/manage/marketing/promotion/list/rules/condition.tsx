import u from '@/utils';
import { PromotionConditionDto } from '@/utils/models';
import { EditOutlined, CloseOutlined } from '@ant-design/icons';
import {
  Button,
  Card,
  Form,
  InputNumber,
  message,
  Modal,
  Select,
  Empty,
} from 'antd';
import { useState } from 'react';
import XConditionDescriptor from '../../components/condition';

export default (props: {
  data: PromotionConditionDto[];
  ok: any;
  remove: any;
}) => {
  const { data, ok, remove } = props;
  const [show, _show] = useState(false);

  const [condition, _condition] = useState<PromotionConditionDto>({});
  const [selectedType, _selectedType] = useState('');

  const callback = () => {
    if (u.isEmpty(condition.ConditionType)) {
      message.error('pls select condition type');
      return;
    }
    ok && ok(condition);
    _show(false);
  };

  const tryRenderOrderPriceRule = () => {
    return (
      selectedType == 'order-price' && (
        <>
          <Form.Item label="订单金额不低于">
            <InputNumber
              defaultValue={0}
              onChange={(e) => {
                _condition({
                  ...condition,
                  ConditionJson: JSON.stringify({
                    LimitedOrderAmount: e,
                  }),
                });
              }}
            />
          </Form.Item>
        </>
      )
    );
  };

  return (
    <>
      <Card
        title="过滤条件"
        extra={
          <Button
            onClick={() => {
              _show(true);
            }}
            icon={<EditOutlined />}
            size="small"
          ></Button>
        }
      >
        {u.isEmpty(data) && <Empty />}
        {data.map((x, index) => (
          <div
            key={index}
            style={{
              margin: 10,
              padding: 10,
              border: '1px dashed gray',
              display: 'flex',
              flexDirection: 'row',
              alignItems: 'center',
              justifyContent: 'space-between',
            }}
          >
            <XConditionDescriptor data={x} />
            <Button
              danger
              icon={<CloseOutlined />}
              onClick={() => {
                remove && remove(index);
              }}
            ></Button>
          </div>
        ))}
      </Card>
      <Modal
        title="编辑条件规则"
        open={show}
        onCancel={() => _show(false)}
        onOk={() => {
          callback();
        }}
      >
        <Select
          style={{ minWidth: 200, marginBottom: 10 }}
          value={selectedType}
          onChange={(e) => {
            _selectedType(e);
            _condition({
              ConditionType: e,
            });
          }}
        >
          <Select.Option key={1} value={'*'}>
            全部
          </Select.Option>
          <Select.Option key={1} value={'order-price'}>
            订单金额不低于
          </Select.Option>
        </Select>
        <Form style={{}}>{tryRenderOrderPriceRule()}</Form>
      </Modal>
    </>
  );
};
