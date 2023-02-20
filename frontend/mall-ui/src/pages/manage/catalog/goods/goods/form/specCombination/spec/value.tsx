import u from '@/utils';
import http from '@/utils/http';
import { DeleteOutlined, PlusOutlined, SaveOutlined } from '@ant-design/icons';
import {
  Button,
  Form,
  Input,
  InputNumber,
  Modal,
  Space,
  Spin,
  Tag,
  Tooltip,
  message,
} from 'antd';
import { useEffect, useState } from 'react';

export default (props: any) => {
  const { model, ok } = props;

  const [data, _data] = useState<any>([]);

  const [showForm, _showForm] = useState(false);
  const [formData, _formData] = useState<any>({});
  const [loadingSave, _loadingSave] = useState(false);

  useEffect(() => {
    _data(model?.Values || []);
  }, [model]);

  const [form] = Form.useForm();

  const save = (row: any) => {
    _loadingSave(true);

    row.GoodsSpecId = model.Id;
    http.apiRequest
      .post('/mall-admin/spec/save-value', {
        ...row,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('保存成功');
          _showForm(false);
          ok && ok();
        }
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  const deleteSpecValue = (record: any) => {
    if (!confirm('确定删除规格')) {
      return;
    }
    _loadingSave(true);
    http.apiRequest
      .post('/mall-admin/spec/delete-value', {
        Id: record.Id,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('删除成功');
          _showForm(false);
          ok && ok();
        }
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  const descriptPriceOffset = (price: number) => {
    if (price > 0) {
      return `+${price}元`;
    } else if (price < 0) {
      return `-${-price}元`;
    } else {
      return `无价差`;
    }
  };

  useEffect(() => {
    form.resetFields();
    if (formData) {
      form.setFieldsValue(formData);
    }
  }, [formData]);

  return (
    <>
      <Modal
        open={showForm}
        onCancel={() => _showForm(false)}
        footer={
          <Space size={2}>
            {formData && formData.Id && (
              <Button
                type="primary"
                danger
                icon={<DeleteOutlined />}
                onClick={() => {
                  deleteSpecValue(formData);
                }}
              >
                删除
              </Button>
            )}
            <Button
              type="primary"
              icon={<SaveOutlined />}
              onClick={() => {
                form.submit();
              }}
            >
              保存
            </Button>
          </Space>
        }
      >
        <Spin spinning={loadingSave}>
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
            <Form.Item
              label="名称"
              name="Name"
              rules={[
                {
                  required: true,
                },
                {
                  max: 20,
                },
              ]}
            >
              <Input />
            </Form.Item>
            <Form.Item
              label="描述"
              name="Description"
              rules={[
                {
                  max: 100,
                },
              ]}
            >
              <Input.TextArea />
            </Form.Item>
            <Form.Item label="差价" name="PriceOffset">
              <InputNumber defaultValue={0} />
            </Form.Item>
          </Form>
        </Spin>
      </Modal>
      <Space size={2}>
        {u.map(data, (x, index) => (
          <Tag
            key={index}
            color={'blue'}
            onClick={() => {
              _formData(x);
              _showForm(true);
            }}
          >
            {`${x.Name}--${descriptPriceOffset(x.PriceOffset)}`}
          </Tag>
        ))}
        <Tooltip title="添加规格明细">
          <Button
            size="small"
            icon={<PlusOutlined />}
            onClick={() => {
              _showForm(true);
              _formData({});
            }}
          ></Button>
        </Tooltip>
      </Space>
    </>
  );
};
