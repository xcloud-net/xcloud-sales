import { Button, Card, message, Select } from 'antd';
import { useEffect, useState } from 'react';

import u from '@/utils';
import http from '@/utils/http';

export default (props: any) => {
  const { data, ok } = props;
  const [loading, _loading] = useState(false);
  const [loadingSave, _loadingSave] = useState(false);
  const [taglist, _taglist] = useState<any[]>([]);

  const [selectedTags, _selectedTags] = useState<any[]>([]);

  const queryTags = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/common/list-tags', {})
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _taglist(res.data.Data || []);
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const save = () => {
    var formData = {
      Id: data.Id,
      TagIds: selectedTags,
    };
    _loadingSave(true);

    http.apiRequest
      .post('/mall-admin/goods/set-goods-tags', {
        ...formData,
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

  useEffect(() => {
    queryTags();
  }, []);

  useEffect(() => {
    _selectedTags(u.map(data.Tags || [], (x) => x.Id));
  }, []);

  return (
    <>
      <Card
        title="商品标签"
        size="small"
        loading={loading || loadingSave}
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
        <Select
          mode="multiple"
          maxTagCount={3}
          style={{ width: 300 }}
          value={selectedTags}
          placeholder="请选择标签"
          onChange={(e) => {
            console.log(e);
            _selectedTags(e);
          }}
        >
          {u.map(taglist, (x) => {
            return (
              <Select.Option key={x.Id} value={x.Id}>
                {x.Name}
              </Select.Option>
            );
          })}
        </Select>
      </Card>
    </>
  );
};
