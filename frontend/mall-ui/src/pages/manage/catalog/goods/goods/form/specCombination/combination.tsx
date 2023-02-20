import XQrcode from '@/components/qrcode';
import u from '@/utils';
import http from '@/utils/http';
import { GoodsCombinationDto, GoodsDto } from '@/utils/models';
import {
  Button,
  Card,
  Form,
  Input,
  Modal,
  Popover,
  Table,
  Tag,
  message,
} from 'antd';
import { ColumnType } from 'antd/es/table';
import React, { useEffect, useState } from 'react';

const App = (props: { model: GoodsDto }) => {
  const { model } = props;

  const [data, _data] = useState<GoodsDto>({});
  const [loading, _loading] = useState(false);
  const [show, _show] = useState(false);
  const [formData, _formData] = useState<GoodsCombinationDto>({});
  const [combination, _combination] = useState<GoodsCombinationDto[]>([]);
  const [loadingSave, _loadingSave] = useState(false);

  const [form] = Form.useForm();

  const formEnter = React.useRef<any>();

  const queryList = () => {
    if (!data || !data.Id) {
      return;
    }
    _loading(true);

    http.apiRequest
      .post('/mall-admin/combination/list-by-goodsid', {
        Id: data.Id,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _combination(res.data.Data || []);
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const save = (row: any) => {
    _loadingSave(true);

    row = {
      ...row,
      GoodsId: data.Id,
    };

    console.log(row);
    http.apiRequest
      .post('/mall-admin/combination/save-v1', {
        ...row,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('保存成功');
          _show(false);
          queryList();
        }
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  useEffect(() => {
    if (model) {
      _data(model);
    }
  }, [model]);

  useEffect(() => {
    setTimeout(() => {
      show && formEnter.current?.focus();
    }, 100);
  }, [show]);

  useEffect(() => {
    queryList();
  }, [data]);

  useEffect(() => {
    form.resetFields();
    if (formData) {
      form.setFieldsValue(formData);
    }
  }, [formData]);

  const columns: ColumnType<GoodsCombinationDto>[] = [
    {
      title: '名称',
      render: (x) => x.Name || '--',
    },
    {
      title: '描述',
      render: (x) => x.Description || '--',
    },
    {
      title: '条码',
      render: (record: GoodsCombinationDto) => {
        return (
          u.isEmpty(record.Sku) || (
            <>
              <Popover
                content={<XQrcode value={record.Sku || ''} height={50} />}
                title="SKU"
              >
                <Tag>{record.Sku}</Tag>
              </Popover>
            </>
          )
        );
      },
    },
    {
      title: '颜色',
      render: (x) => x.Color || '--',
    },
    {
      title: '操作',
      width: 200,
      render: (record: GoodsCombinationDto) => {
        return (
          <Button.Group size="small">
            <Button
              type="primary"
              onClick={() => {
                _formData(record);
                _show(true);
              }}
            >
              编辑
            </Button>
          </Button.Group>
        );
      },
    },
  ];

  return (
    <>
      <Modal
        title={`规格`}
        forceRender
        open={show}
        confirmLoading={loadingSave}
        onCancel={() => _show(false)}
        onOk={() => form.submit()}
      >
        <Form
          form={form}
          onFinish={(e) => save(e)}
          labelCol={{ flex: '110px' }}
          labelAlign="right"
          wrapperCol={{ flex: 1 }}
        >
          <Form.Item name="Id" style={{ display: 'none' }}>
            <Input type="hidden" />
          </Form.Item>
          <Form.Item name="GoodsId" style={{ display: 'none' }}>
            <Input type="hidden" />
          </Form.Item>
          <Form.Item
            label="Sku"
            name="Sku"
            rules={[
              {
                max: 20,
              },
            ]}
          >
            <Input ref={formEnter} placeholder="请用扫码前输入商品条码" />
          </Form.Item>
          <Form.Item
            label="名称"
            name="Name"
            rules={[
              {
                required: true,
                message: '不能为空',
              },
              {
                max: 20,
              },
            ]}
          >
            <Input placeholder="请输入商品规格，比如【红色，120ml】" />
          </Form.Item>
          <Form.Item label="描述" name="Description" rules={[{ max: 50 }]}>
            <Input.TextArea />
          </Form.Item>
        </Form>
      </Modal>
      <Card
        title="简单规格"
        size="small"
        extra={
          <Button
            type="primary"
            size="small"
            onClick={() => {
              _show(true);
              _formData({ CostPrice: 0, Price: 0, StockQuantity: 10 });
            }}
          >
            添加
          </Button>
        }
      >
        <Table
          size="small"
          loading={loading}
          columns={columns}
          dataSource={combination}
          pagination={false}
        />
      </Card>
    </>
  );
};

export default App;
