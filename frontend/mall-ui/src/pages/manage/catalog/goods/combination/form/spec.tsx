import {
  GoodsCombinationDto,
  GoodsSpecCombinationItemDto,
  GoodsSpecDto,
} from '@/utils/models';
import { Button, Space, Modal, Select, message, Alert } from 'antd';
import XSpecCombinationDescription from './specCombinationDescription';
import { useEffect, useState } from 'react';
import u from '@/utils';

const App = (props: { model: GoodsCombinationDto; ok: any }) => {
  const { model, ok } = props;

  const [show, _show] = useState(false);
  const [loading, _loading] = useState(false);
  const [specItems, _specItems] = useState<GoodsSpecCombinationItemDto[]>([]);
  const [specs, _specs] = useState<GoodsSpecDto[]>([]);

  const AllSpecCombinationItems: GoodsSpecCombinationItemDto[] = specs
    .flatMap((x) => x.Values || [])
    .map((x) => ({
      SpecId: x.GoodsSpecId,
      SpecValueId: x.Id,
    }));

  const fingerPrint = (x: GoodsSpecCombinationItemDto) =>
    `${x.SpecId}=${x.SpecValueId}`;

  const querySpecs = () => {
    if (!model.GoodsId) {
      return;
    }
    _loading(true);
    u.http.apiRequest
      .post('/mall-admin/spec/list', {
        Id: model.GoodsId,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          _specs(res.data.Data || []);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  const saveSpecs = () => {
    _loading(true);

    var allFingerPrints = AllSpecCombinationItems.map((x) => fingerPrint(x));
    var validSpecItems = specItems.filter(
      (x) => allFingerPrints.indexOf(fingerPrint(x)) >= 0,
    );

    u.http.apiRequest
      .post('/mall-admin/combination/update-spec-combination', {
        Id: model.Id,
        ParsedSpecificationsJson: validSpecItems,
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
    show && querySpecs();
  }, [show]);

  useEffect(() => {
    model && _specItems(model.ParsedSpecificationsJson || []);
  }, [model]);

  return (
    <>
      <Modal
        open={show}
        title="规格组合"
        confirmLoading={loading}
        onCancel={() => {
          _show(false);
        }}
        onOk={() => {
          saveSpecs();
        }}
        okText="保存"
      >
        {loading ||
          (u.isEmpty(specs) && <Alert message="请先配置规格"></Alert>)}
        {specs.map((x, i) => {
          const items = x.Values || [];

          const selectedValueId = specItems.find(
            (d) => d.SpecId == x.Id,
          )?.SpecValueId;

          return (
            <div key={i} style={{ marginBottom: 10 }}>
              <Space direction="horizontal">
                <span>{x.Name || '--'}</span>
                <Select
                  onChange={(e: number) => {
                    var otherSpecs = specItems.filter((d) => d.SpecId != x.Id);
                    if (e <= 0) {
                      _specItems([...otherSpecs]);
                    } else {
                      _specItems([
                        ...otherSpecs,
                        {
                          SpecId: x.Id,
                          SpecValueId: e,
                        },
                      ]);
                    }
                  }}
                  value={selectedValueId || -1}
                  style={{ width: 300 }}
                >
                  <Select.Option key={-1} value={-1}>
                    请选择
                  </Select.Option>
                  {items.map((m, index) => (
                    <Select.Option key={index} value={m.Id}>
                      {m.Name || '--'}
                    </Select.Option>
                  ))}
                </Select>
              </Space>
            </div>
          );
        })}
      </Modal>
      <div
        style={{
          backgroundColor: `rgb(240,240,240)`,
          padding: 5,
        }}
      >
        <Space direction="horizontal">
          <XSpecCombinationDescription model={model} />
          <Button
            type="primary"
            size="small"
            onClick={() => {
              _show(true);
            }}
          >
            修改
          </Button>
        </Space>
      </div>
    </>
  );
};

export default App;
