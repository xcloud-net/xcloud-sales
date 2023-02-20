import http from '@/utils/http';
import { Button, Card, Form, Input, InputNumber, message, Modal, Table } from 'antd';
import { ColumnType } from 'antd/es/table';
import React, { useState } from 'react';

const add_dict_item = async (data: {}) => {
  var res = await http.adminRequest.post('/dict/add-item', {
    ...data,
  });
  return res.data;
};

const delete_dict_item = async (id: string) => {
  var res = await http.adminRequest.post('/dict/delete-item', {
    ...{ Id: id },
  });
  return res.data;
};

const dictPage = (props: any) => {
  const { selectedData, onSuccessSave } = props;

  const [loadingSaveItem, _loadingSaveItem] = useState(false);
  const [showItemModal, _showItemModal] = useState(false);
  const [itemForm] = Form.useForm();

  const selectedDictItems = selectedData?.DictItems || [];

  const valColumns: ColumnType<any>[] = [
    {
      title: '名称',
      dataIndex: 'ItemName',
    },
    {
      title: '字典值',
      dataIndex: 'ItemValue',
    },
    {
      title: '排序',
      dataIndex: 'Sort',
    },
    {
      title: '操作',
      render: (row: any) => {
        return (
          <Button.Group size="small">
            <Button
              type="link"
              danger
              onClick={async () => {
                if (!confirm('delete?')) {
                  return;
                }
                await delete_dict_item(row.Id || '');
                message.success('success');
                onSuccessSave();
              }}
            >
              删除
            </Button>
          </Button.Group>
        );
      },
    },
  ];
  return (
    <>
      <Card
        extra={
          <Button
            disabled={!(selectedData && selectedData.Id)}
            size="small"
            type="primary"
            onClick={() => {
              _showItemModal(true);
            }}
          >
            添加
          </Button>
        }
      >
        <Table
          pagination={false}
          size="small"
          columns={valColumns}
          rowKey={(x) => x.Id}
          dataSource={selectedDictItems}
        />
      </Card>

      <Modal
        visible={showItemModal}
        footer={false}
        title="字典数据"
        onCancel={() => {
          _showItemModal(false);
        }}
      >
        <Form
          form={itemForm}
          labelCol={{ span: 6 }}
          wrapperCol={{ span: 18 }}
          onFinish={async (e) => {
            e.DictId = selectedData.Id;
            _loadingSaveItem(true);
            add_dict_item(e)
              .then((res) => {
                message.success('添加成功');
                _showItemModal(false);
                onSuccessSave();
              })
              .finally(() => {
                _loadingSaveItem(false);
              });
          }}
        >
          <Form.Item label="字典名" name="ItemName" rules={[{ required: true, message: '请输入' }]}>
            <Input />
          </Form.Item>
          <Form.Item
            label="字典值"
            name="ItemValue"
            rules={[{ required: true, message: '请输入' }]}
          >
            <Input />
          </Form.Item>
          <Form.Item label="排序" name="Sort" rules={[{ required: true, message: '请输入' }]}>
            <InputNumber />
          </Form.Item>
          <Button block type="primary" htmlType="submit" loading={loadingSaveItem}>
            保存
          </Button>
        </Form>
      </Modal>
    </>
  );
};

export default dictPage;
