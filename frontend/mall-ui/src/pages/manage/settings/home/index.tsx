import SampleData from '@/components/xpage/data';
import u from '@/utils';
import http from '@/utils/http';
import { Button, Card, Col, Row, Space, message, Switch } from 'antd';
import { useEffect, useState } from 'react';
import XJson from './json';
import XPreview from './preview';

export default function () {
  const [data, _data] = useState({
    Published: false,
    Blocks: '',
  });

  const [loading, _loading] = useState(false);
  const [loadingSave, _loadingSave] = useState(false);

  const [originJsonData, _originJsonData] = useState({});
  const [jsonData, _jsonData] = useState({});

  const queryData = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/setting/home-blocks')
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data.Data || {});
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  const save = () => {
    _loadingSave(true);
    http.apiRequest
      .post('/mall-admin/setting/save-home-blocks', {
        Blocks: JSON.stringify(jsonData),
      })
      .then((res) => {
        u.handleResponse(res, () => {
          message.success('保存成功');
          queryData();
        });
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  const parseJsonObject = () => {
    try {
      if (u.isEmpty(data.Blocks)) {
        return {};
      }
      var jsonModel = JSON.parse(data.Blocks || '{}');
      return jsonModel;
    } catch (e) {
      console.log(e, 'set body content');
    }

    return {};
  };

  useEffect(() => {
    const parsedData = parseJsonObject();
    _originJsonData(parsedData);
    _jsonData(parsedData);
  }, [data.Blocks]);

  useEffect(() => {
    queryData();
  }, []);

  return (
    <>
      <Card
        loading={loading}
        title="首页设计"
        extra={
          <Space direction="horizontal">
            <Button
              onClick={() => {
                _data({
                  ...data,
                  Blocks: JSON.stringify(SampleData),
                });
              }}
            >
              导入测试数据
            </Button>
            <Button
              onClick={() => {
                save();
              }}
              type="primary"
              loading={loadingSave}
            >
              保存
            </Button>
          </Space>
        }
      >
        <Row gutter={10}>
          <Col span={14}>
            <div
              style={{
                marginBottom: 10,
              }}
            >
              <span>发布？</span>
              <Switch
                checked={data.Published}
                onChange={(e) => {
                  _data({
                    ...data,
                    Published: e,
                  });
                }}
              />
            </div>
            <div style={{ height: 600 }}>
              <XJson
                json={originJsonData}
                ok={(e: string) => {
                  try {
                    console.log(e);
                    _jsonData(JSON.parse(e));
                  } catch (err) {
                    console.log(err);
                  }
                }}
              />
            </div>
          </Col>
          <Col span={10}>
            <div
              style={{
                display: 'flex',
                flexDirection: 'row',
                alignItems: 'center',
                justifyContent: 'center',
              }}
            >
              <XPreview data={jsonData} />
            </div>
          </Col>
        </Row>
      </Card>
    </>
  );
}
