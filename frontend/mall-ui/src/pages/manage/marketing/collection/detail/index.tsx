import http from '@/utils/http';
import u from '@/utils';
import {
  Form,
  Input,
  InputNumber,
  message,
  Modal,
  Spin,
  Table,
  Button,
  Card,
  Image,
} from 'antd';
import { ColumnType } from 'antd/es/table';
import { useEffect, useState } from 'react';
import XSearch from './goodsSelector';
import { GoodsDto } from '@/utils/models';

export default (props: any) => {
  const { data, ok } = props;
  const [loading, _loading] = useState(false);
  const [show, _show] = useState(false);
  const [selectedGoods, _selectedGoods] = useState<any>(null);
  const [form] = Form.useForm();

  const save = (row: any) => {
    _loading(true);

    http.apiRequest
      .post('/mall-admin/collection/add-goods', {
        ...row,
        CollectionId: data.Id,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('保存成功');
          _show(false);
          ok();
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const removeGoods = (id: any) => {
    _loading(true);

    http.apiRequest
      .post('/mall-admin/collection/remove-goods', {
        Id: id,
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

  const columns: ColumnType<any>[] = [
    {
      title: '商品主图',
      render: (x) => {
        const goods: GoodsDto = x.Goods;
        var meta = u.first(goods.XPictures || []);
        if (!meta) {
          return null;
        }
        var url = u.resolveUrlv2(meta, {
          width: 100,
          height: 100,
        });
        return (
          <>
            <Image src={url || ''} style={{ width: '50px', height: '50px' }} />
          </>
        );
      },
    },
    {
      title: '商品名称',
      render: (x) => `${x.Goods?.Name}-${x.GoodsSpecCombination?.Name}`,
    },
    {
      title: '单价',
      render: (x) => x.GoodsSpecCombination?.Price,
    },
    {
      title: '数量',
      render: (x) => x.Quantity,
    },
    {
      title: '操作',
      render: (x) => (
        <Button.Group>
          <Button type="primary" danger onClick={() => removeGoods(x.Id)}>
            删除
          </Button>
        </Button.Group>
      ),
    },
  ];

  useEffect(() => {
    var obj = {
      GoodsId: 0,
      GoodsSpecCombinationId: 0,
    };
    if (selectedGoods) {
      obj.GoodsId = selectedGoods.GoodsId;
      obj.GoodsSpecCombinationId = selectedGoods.Id;
    }
    form.setFieldsValue(obj);
  }, [selectedGoods]);

  return (
    <>
      <Card
        bordered={false}
        size="small"
        extra={
          <Button
            size="small"
            onClick={() => {
              _show(true);
            }}
          >
            添加商品
          </Button>
        }
      >
        <Table
          size="small"
          columns={columns}
          dataSource={data.Items}
          pagination={false}
        />
      </Card>
      <Modal
        title="添加商品"
        visible={show}
        onCancel={() => _show(false)}
        onOk={() => form.submit()}
      >
        <Spin spinning={loading}>
          <XSearch
            model={data}
            selectedGoods={selectedGoods}
            onSelect={(x: any) => {
              if (selectedGoods?.Id == x.Id) {
                _selectedGoods(null);
              } else {
                _selectedGoods(x);
              }
            }}
            onClear={() => {
              _selectedGoods(null);
            }}
          />

          <Form
            form={form}
            onFinish={(e) => save(e)}
            labelCol={{ flex: '110px' }}
            labelAlign="right"
            wrapperCol={{ flex: 1 }}
          >
            <Form.Item name="Id" hidden>
              <Input />
            </Form.Item>
            <Form.Item name="GoodsId" hidden>
              <Input />
            </Form.Item>
            <Form.Item name="GoodsSpecCombinationId" hidden>
              <Input />
            </Form.Item>
            <Form.Item
              label="描述"
              name="Quantity"
              rules={[
                {
                  required: true,
                },
              ]}
            >
              <InputNumber defaultValue={1} />
            </Form.Item>
          </Form>
        </Spin>
      </Modal>
    </>
  );
};
