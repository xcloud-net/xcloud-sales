import http from '@/utils/http';
import {
  StockDto,
  StockItemDto,
  SupplierDto,
  WarehouseDto,
} from '@/utils/models';
import { DatePicker, Form, Input, Modal, Select, message } from 'antd';
import { useEffect, useState } from 'react';
import XItems from './items';
import u from '@/utils';

export default (props: {
  show: boolean;
  hide: any;
  data: StockDto;
  ok: any;
}) => {
  const { show, hide, data, ok } = props;
  const [loading, _loading] = useState(false);
  const [items, _items] = useState<StockItemDto[]>([]);
  const [warehouses, _warehouses] = useState<WarehouseDto[]>([]);
  const [suppliers, _suppliers] = useState<SupplierDto[]>([]);

  const [form] = Form.useForm<StockDto>();

  const checkGoodsInput = (m: StockItemDto) => {
    if (
      m.CombinationId &&
      m.CombinationId > 0 &&
      m.Quantity &&
      m.Quantity > 0 &&
      m.Price &&
      m.Price > 0
    ) {
    } else {
      return false;
    }

    return true;
  };

  const save = (row: StockDto) => {
    console.log(row);

    if (items.length <= 0) {
      message.error('请添加商品');
      return;
    }
    if (items.some((x) => !checkGoodsInput(x))) {
      message.error('请输入采购信息');
      return;
    }

    _loading(true);
    http.apiRequest
      .post('/mall-admin/stock/insert-stock', {
        ...row,
        ExpirationTime: u.formatDateTime(row.ExpirationTime || ''),
        Items: items,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          message.success('保存成功');
          ok();
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  const loadData = () => {
    u.http.apiRequest.post('/mall-admin/supplier/all').then((res) => {
      u.handleResponse(res, () => {
        _suppliers(res.data.Data || []);
      });
    });

    u.http.apiRequest.post('/mall-admin/warehouse/all').then((res) => {
      u.handleResponse(res, () => {
        _warehouses(res.data.Data || []);
      });
    });
  };

  useEffect(() => {
    show && loadData();
    show && _items([]);
  }, [show]);

  useEffect(() => {
    form.resetFields();
    if (data) {
      var copy = { ...data };
      if (u.isEmpty(copy.SupplierId)) {
        copy.SupplierId = undefined;
      }
      if (u.isEmpty(copy.WarehouseId)) {
        copy.WarehouseId = undefined;
      }
      form.setFieldsValue(copy);
    }
  }, [data]);

  return (
    <>
      <Modal
        confirmLoading={loading}
        open={show}
        onCancel={() => hide()}
        onOk={() => form.submit()}
        width={'90%'}
        title="添加采购单"
        forceRender
        destroyOnClose
      >
        <Form
          form={form}
          onFinish={(e) => save(e)}
          labelCol={{ flex: '110px' }}
          labelAlign="right"
          wrapperCol={{ flex: 1 }}
        >
          <Form.Item name="Id">
            <Input type="hidden" />
          </Form.Item>
          <Form.Item label="供应商" name="SupplierId">
            <Select allowClear placeholder="请选择供货商">
              {suppliers.map((x, i) => (
                <Select.Option key={i} value={x.Id}>
                  {x.Name || '--'}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item label="目标仓库" name="WarehouseId">
            <Select allowClear placeholder="请选择目标仓库">
              {warehouses.map((x, i) => (
                <Select.Option key={i} value={x.Id}>
                  {x.Name || '--'}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item label="过期时间" name="ExpirationTime">
            <DatePicker />
          </Form.Item>
          <Form.Item label="备注" name="Remark" rules={[{ max: 500 }]}>
            <Input.TextArea placeholder="请输入采购备注" />
          </Form.Item>
        </Form>
        {show && (
          <XItems
            onChange={(e: StockItemDto[]) => {
              _items(e);
            }}
          />
        )}
      </Modal>
    </>
  );
};
