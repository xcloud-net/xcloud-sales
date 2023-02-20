import http from '@/utils/http';
import { Alert, Card, message, Modal, Spin, Tabs } from 'antd';
import { useEffect, useState } from 'react';
import XForm from './form';
import XOther from './other';
import XPictures from './pictures';
import XSpecs from './specCombination';
import XAttribute from './attr';
import { GoodsDto } from '@/utils/models';

export default (props: { goodsId: number; ok: any; hide: any; show: any }) => {
  const { goodsId, ok, hide, show } = props;

  const [id, _id] = useState(0);
  const [goods, _goods] = useState<GoodsDto>({});
  const [tab, _tab] = useState('1');
  const [loading, _loading] = useState(false);

  const queryGoods = () => {
    if (!id || id <= 0) {
      _goods({});
      return;
    }

    _loading(true);

    http.apiRequest
      .post('/mall-admin/goods/by-id', {
        Id: id,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _goods(res.data.Data || {});
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const renderWithCheck = (children: any) => {
    if (!goods || !goods.Id) {
      return (
        <Card style={{ margin: 10 }}>
          <Alert message="请先保存商品再编辑此模块"></Alert>
        </Card>
      );
    }

    return <>{children}</>;
  };

  useEffect(() => {
    show && _tab('1');
  }, [show]);

  useEffect(() => {
    _id(goodsId);
  }, [goodsId]);

  useEffect(() => {
    queryGoods();
  }, [id]);

  return (
    <>
      <Modal
        title="编辑商品"
        forceRender
        open={show}
        onCancel={() => {
          hide && hide();
        }}
        onOk={() => {}}
        footer={false}
        width={'90%'}
      >
        <Spin spinning={loading}>
          <Tabs
            activeKey={tab}
            onChange={(e) => {
              _tab(e);
            }}
            size="small"
            destroyInactiveTabPane
          >
            <Tabs.TabPane tab="基本信息" key="1">
              <XForm
                show={show}
                data={goods}
                ok={(g: any) => {
                  if (g.Id == goods.Id) {
                    queryGoods();
                  } else {
                    _id(g.Id);
                  }
                }}
              />
            </Tabs.TabPane>
            <Tabs.TabPane tab="商品属性" key="12">
              {renderWithCheck(
                <XAttribute
                  model={goods}
                  ok={() => {
                    queryGoods();
                  }}
                />,
              )}
            </Tabs.TabPane>
            <Tabs.TabPane tab="规格明细" key="2">
              {renderWithCheck(
                <XSpecs
                  model={goods}
                  ok={() => {
                    queryGoods();
                  }}
                />,
              )}
            </Tabs.TabPane>
            <Tabs.TabPane tab="商品图片" key="3">
              {renderWithCheck(
                <>
                  <XPictures
                    data={goods}
                    ok={() => {
                      queryGoods();
                    }}
                  />
                </>,
              )}
            </Tabs.TabPane>
            <Tabs.TabPane tab="其他" key="6">
              {renderWithCheck(
                <XOther
                  data={goods}
                  ok={() => {
                    queryGoods();
                  }}
                />,
              )}
            </Tabs.TabPane>
          </Tabs>
        </Spin>
      </Modal>
    </>
  );
};
