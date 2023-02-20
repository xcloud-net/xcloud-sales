import u from '@/utils';
import http from '@/utils/http';
import { DeleteFilled, EditFilled, PlusOutlined } from '@ant-design/icons';
import { Button, Card, message, Spin, Tooltip } from 'antd';
import React, { useEffect, useState } from 'react';
import XForm from './form';
import XItem from './detail';
import { Box } from '@mui/material';

export default (props: any): React.ReactNode => {
  const [loading, _loading] = useState(true);
  const [data, _data] = useState<any>([]);

  const [showForm, _showForm] = useState(false);
  const [formData, _formData] = useState({});

  const [loadingId, _loadingId] = useState(0);

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/collection/all', {})
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _data(res.data.Data || []);
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const deleteRow = (row: any) => {
    if (!confirm('删除？')) {
      return;
    }
    _loadingId(row.Id);
    http.apiRequest
      .post('/mall-admin/collection/hide', {
        Id: row.Id,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('删除成功');
          queryList();
        }
      })
      .finally(() => {
        _loadingId(0);
      });
  };

  useEffect(() => {
    queryList();
  }, []);

  return (
    <>
      <XForm
        show={showForm}
        hide={() => _showForm(false)}
        data={formData}
        ok={() => {
          _showForm(false);
          queryList();
        }}
      />
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'row',
          alignItems: 'right',
          justifyContent: 'flex-end',
        }}
      >
        <Box sx={{}}>
          <Button
            type="dashed"
            icon={<PlusOutlined />}
            onClick={() => {
              _formData({});
              _showForm(true);
            }}
            style={{
              marginBottom: 10,
            }}
          >
            新增组合
          </Button>
        </Box>
      </Box>
      {loading && <Spin spinning />}
      {u.map(data, (x, index) => {
        return (
          <Card
            size="small"
            key={index}
            title={x.Name}
            extra={
              <Button.Group size="small">
                <Tooltip title="编辑">
                  <Button
                    icon={<EditFilled />}
                    type="primary"
                    onClick={() => {
                      _formData(x);
                      _showForm(true);
                    }}
                  ></Button>
                </Tooltip>
                <Tooltip title="删除">
                  <Button
                    icon={<DeleteFilled />}
                    loading={loadingId == x.Id}
                    type="primary"
                    danger
                    onClick={() => {
                      deleteRow(x);
                    }}
                  ></Button>
                </Tooltip>
              </Button.Group>
            }
            style={{
              marginBottom: 10,
            }}
          >
            <p>{x.Description}</p>
            <XItem
              data={x}
              ok={() => {
                queryList();
              }}
            />
          </Card>
        );
      })}
    </>
  );
};
