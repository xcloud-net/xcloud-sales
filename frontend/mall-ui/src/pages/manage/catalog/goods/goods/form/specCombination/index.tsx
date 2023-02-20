import u from '@/utils';
import { Card, Switch } from 'antd';
import { useEffect, useState } from 'react';

import XCombination from './combination';
import XSpecification from './spec';
import { GoodsDto } from '@/utils/models';

export default (props: { model: GoodsDto; ok: any }) => {
  const { model, ok } = props;

  const [key, _key] = useState('1');

  useEffect(() => {
    var k = u.toString(model.AttributeType);
    if (['1', '2'].indexOf(k) >= 0) {
      _key(k);
    }
  }, [model]);

  return (
    <>
      <Card
        style={{ display: 'none' }}
        size="small"
        extra={<Switch title="使用简单规格" />}
      ></Card>
      <XSpecification model={model} />
      <XCombination model={model} />
    </>
  );
};
