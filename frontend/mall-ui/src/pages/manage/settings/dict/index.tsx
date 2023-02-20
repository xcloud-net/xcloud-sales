import http from '@/utils/http';
import { Button, Card, Col, Empty, Menu, message, Modal, Row, Space } from 'antd';
import React, { useEffect, useState } from 'react';
import AddForm from './AddForm';
import DictItem from './DictItem';

const all_dict = async () => {
  var res = await http.adminRequest.post('/dict/list');
  return res.data;
};

const delete_dict = async (id: string) => {
  var res = await http.adminRequest.post('/dict/delete', {
    ...{ Id: id },
  });
  return res.data;
};

const dictPage = (props: any) => {
  const [loading, _loading] = useState(false);

  const [showDictModal, _showDictModal] = useState(false);
  const [dictFromStore, _dictFromStore] = useState<any[]>([]);
  const [selectedDictItems, _selectedDictItems] = useState<any>({});

  const [formData, _formData] = useState<any>({});

  const hasSelected = selectedDictItems && selectedDictItems.Id;

  const queryAll = async () => {
    _loading(true);
    all_dict()
      .then((res) => {
        _dictFromStore(res.Data || []);
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryAll();
  }, []);

  return (
    <>
      <Row gutter={20}>
        <Col span={8}>
          <Card
            title="字典"
            size="small"
            loading={loading}
            extra={
              <Space>
                <Button
                  size="small"
                  type="primary"
                  onClick={async () => {
                    await queryAll();
                  }}
                >
                  刷新
                </Button>
                <Button
                  size="small"
                  type="primary"
                  onClick={() => {
                    _showDictModal(true);
                    _formData({});
                  }}
                >
                  添加
                </Button>
                <Button
                  size="small"
                  type="primary"
                  danger
                  disabled={!hasSelected}
                  onClick={async () => {
                    if (!confirm('delete?')) {
                      return;
                    }
                    await delete_dict(selectedDictItems.Id || '');
                    message.success('success');
                    await queryAll();
                  }}
                >
                  {`删除${selectedDictItems.Name || ''}`}
                </Button>
              </Space>
            }
          >
            {dictFromStore && dictFromStore.length > 0 ? (
              <Menu
                activeKey={selectedDictItems.Id || ''}
                selectedKeys={[selectedDictItems.Id || '']}
              >
                {dictFromStore.map((x: any) => (
                  <Menu.Item
                    key={x.Id}
                    onClick={() => {
                      _selectedDictItems(x);
                    }}
                  >
                    {x.Name + `(${x.DictItems?.length ?? 0})`}
                  </Menu.Item>
                ))}
              </Menu>
            ) : (
              <Empty />
            )}
          </Card>
          <Modal
            visible={showDictModal}
            footer={false}
            title="字典数据"
            onCancel={() => {
              _showDictModal(false);
            }}
          >
            <AddForm
              formData={formData}
              onSuccessSave={() => {
                _showDictModal(false);
                _formData({});
                queryAll();
              }}
            />
          </Modal>
        </Col>
        <Col span={16}>
          <DictItem
            selectedData={selectedDictItems}
            onSuccessSave={() => {
              queryAll();
            }}
          />
        </Col>
      </Row>
    </>
  );
};

export default dictPage;
