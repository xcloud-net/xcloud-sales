import http from '@/utils/http';
import {
  message,
  Modal,
  Select,
  Spin
} from 'antd';
import React, { useEffect, useState } from 'react';

export default (props: any) => {
  const { show, hide, data, ok } = props;
  const [loading, _loading] = useState(false);

  const [allStores, _allStores] = useState([]);
  const [selectedStores, _selectedStores] = useState([]);

  const load = () => {
    _loading(true);

    http.apiRequest
      .post('/mall-admin/store/list', {
        //data: row,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _allStores(res.data.Data || []);
          http.apiRequest
            .post('/mall-admin/goods/store-mapping-list', { Id: data })
            .then((mappingRes) => {
              if (mappingRes.data.Error) {
                message.error(mappingRes.data.Error.Message);
              } else {
                var ids = (mappingRes.data.Data || []).map((d: any) => d.Id);
                _selectedStores(ids);
              }
            });
        }
      })
      .finally(() => {
        _loading(false);
      });
  };
  const save = () => {
    _loading(true);

    http.apiRequest
      .post('/mall-admin/goods/save-store-mapping', {
        GoodsId: data,
        Mapping: selectedStores.map((x) => ({
          StoreId: x,
        })),
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('保存成功');
          ok();
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    if (data > 0) {
      load();
    }
  }, [data]);

  return (
    <>
      <Modal title="选择在售门店" visible={show} onCancel={() => hide()} onOk={() => save()}>
        <Spin spinning={loading}>
          <Select
            value={selectedStores}
            mode="multiple"
            style={{ width: '100%' }}
            placeholder="Please select"
            onChange={(value) => _selectedStores(value)}
          >
            {allStores.map((x: any) => (
              <Select.Option value={x.Id}>{x.StoreName}</Select.Option>
            ))}
          </Select>
        </Spin>
      </Modal>
    </>
  );
};
