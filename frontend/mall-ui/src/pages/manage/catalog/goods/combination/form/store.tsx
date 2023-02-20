import u from '@/utils';
import http from '@/utils/http';
import { GoodsCombinationDto, StoreDto } from '@/utils/models';
import { Modal, Select, message, Button, Space } from 'antd';
import { useEffect, useState } from 'react';
import { EditOutlined } from '@ant-design/icons';

export default (props: { data: GoodsCombinationDto; ok: any }) => {
  const { data, ok } = props;
  const [loading, _loading] = useState(false);
  const [show, _show] = useState(false);

  const [allStores, _allStores] = useState<StoreDto[]>([]);
  const [selectedStores, _selectedStores] = useState<string[]>([]);

  const load = () => {
    if (!data.Id || data.Id <= 0) {
      return;
    }
    _loading(true);

    http.apiRequest
      .post('/mall-admin/store/list', {})
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _allStores(res.data.Data || []);
        }
      })
      .finally(() => {
        _loading(false);
      });

    http.apiRequest
      .post('/mall-admin/combination/store-mapping-list', { Id: data.Id })
      .then((res) => {
        u.handleResponse(res, () => {
          var ids = (res.data.Data || []).map((d: any) => d.Id);
          _selectedStores(ids);
        });
      });
  };

  const save = () => {
    _loading(true);

    http.apiRequest
      .post('/mall-admin/combination/save-store-mapping', {
        GoodsCombinationId: data.Id,
        StoreIds: selectedStores,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          message.success('保存成功');
          _show(false);
          ok && ok();
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    show && load();
  }, [show]);

  return (
    <>
      <Space direction="horizontal">
        <div>
          {u.isEmpty(data.Stores) && <span>--</span>}
          {u.isEmpty(data.Stores) || (
            <span>
              {(data.Stores || []).map((x) => x.StoreName || '--').join(',')}
            </span>
          )}
        </div>
        <Button
          icon={<EditOutlined />}
          onClick={() => {
            _show(true);
          }}
        ></Button>
      </Space>
      <Modal
        title="在售门店"
        confirmLoading={loading}
        open={show}
        okText="保存"
        onCancel={() => {
          _show(false);
        }}
        onOk={() => {
          save();
        }}
      >
        <Select
          value={selectedStores}
          mode="multiple"
          style={{ width: '100%' }}
          placeholder="Please select"
          onChange={(value) => _selectedStores(value)}
        >
          {allStores.map((x: StoreDto) => (
            <Select.Option value={x.Id}>{x.StoreName}</Select.Option>
          ))}
        </Select>
      </Modal>
    </>
  );
};
