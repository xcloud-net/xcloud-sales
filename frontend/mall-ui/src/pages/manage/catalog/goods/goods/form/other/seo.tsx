import { Button, Card, Input, message } from 'antd';
import { useEffect, useState } from 'react';

import u from '@/utils';
import http from '@/utils/http';
import pinyin from 'pinyin';

export default (props: any) => {
  const { data, ok } = props;
  const [loadingSave, _loadingSave] = useState(false);

  const [name, _name] = useState('');

  const save = () => {
    _loadingSave(true);

    http.apiRequest
      .post('/mall-admin/goods/set-seo-name', {
        GoodsId: data.Id,
        SeoName: name,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('保存成功');
          ok && ok();
        }
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  const convertSeoName = (name: any) => {
    if (u.isEmpty(name)) {
      return '';
    }
    var py: any = pinyin(name, {
      style: pinyin.STYLE_NORMAL,
    });
    py = u.flatMap(py, (x) => x);
    //concat
    var SeoName = py.join('-').toLowerCase();
    return SeoName;
  };

  useEffect(() => {
    _name(data.SeoName || '');
  }, [data]);

  return (
    <>
      <Card
        title={`${data.Name}-Seo Name`}
        loading={loadingSave}
        style={{
          marginBottom: 10,
        }}
        extra={
          <Button
            type="primary"
            size="small"
            onClick={() => {
              save();
            }}
          >
            保存
          </Button>
        }
      >
        <Input.Group size="large">
          <Input
            style={{ width: 'calc(100% - 200px)' }}
            value={name}
            onChange={(e) => _name(e.target.value)}
            addonBefore={'https://xx.com/goods/'}
          />
          <Button
            size="large"
            onClick={() => {
              data.Name && _name(convertSeoName(data.Name));
            }}
          >
            使用产品名称拼音
          </Button>
        </Input.Group>
      </Card>
    </>
  );
};
