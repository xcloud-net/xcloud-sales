import XQrcode from '@/components/qrcode';
import u from '@/utils';
import { GoodsCombinationDto } from '@/utils/models';
import { Descriptions, Tag } from 'antd';

export default (props: { model: GoodsCombinationDto }) => {
  const { model } = props;
  const { Goods } = model;

  return (
    <>
      <Descriptions bordered>
        <Descriptions.Item label="商品">
          {Goods?.Name || '--'}
        </Descriptions.Item>
        <Descriptions.Item label="规格">{model.Name || '--'}</Descriptions.Item>
        <Descriptions.Item label="商品码">
          {u.isEmpty(model.Sku) || (
            <XQrcode value={model.Sku || ''} height={50} />
          )}
          {u.isEmpty(model.Sku) && <span>未录入sku</span>}
        </Descriptions.Item>
        <Descriptions.Item label="库存">
          {model.StockQuantity || 0}
        </Descriptions.Item>
        <Descriptions.Item label="成本价">
          {model.CostPrice || 0}
        </Descriptions.Item>
        <Descriptions.Item label="零售价">{model.Price}</Descriptions.Item>
        <Descriptions.Item label="状态可用" span={2}>
          {model.IsActive && <Tag color="green">yes</Tag>}
          {model.IsActive || <Tag color="red">no</Tag>}
        </Descriptions.Item>
        <Descriptions.Item label="是否被删除" span={2}>
          {model.IsDeleted && <Tag color="red">yes</Tag>}
          {model.IsDeleted || <Tag color="green">no</Tag>}
        </Descriptions.Item>
        <Descriptions.Item label="描述" span={3}>
          {model.GoodsShortDescription || '--'}
        </Descriptions.Item>
      </Descriptions>
    </>
  );
};
