import http from '@/utils/http';
import { DeleteFilled, EditFilled } from '@ant-design/icons';
import { Button, Card, message, Table } from 'antd';
import { ColumnProps } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import BrandForm from './form';
import XTime from '@/components/manage/time';
import { TagDto } from '@/utils/models';

export default (): React.ReactNode => {
  const [loading, _loading] = useState(true);
  const [data, _data] = useState<TagDto[]>([]);

  const [showForm, _showForm] = useState(false);
  const [formData, _formData] = useState<TagDto>({});

  const [loadingId, _loadingId] = useState(0);

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/common/list-tags', {})
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

  const deleteBrand = (row: any) => {
    if (!confirm('删除？')) {
      return;
    }
    _loadingId(row.Id);
    http.apiRequest
      .post('/mall-admin/common/update-tag-status', {
        Id: row.Id,
        IsDeleted: true,
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

  const columns: ColumnProps<any>[] = [
    {
      title: '名称',
      dataIndex: 'Name',
    },
    {
      title: '描述',
      dataIndex: 'Description',
    },
    {
      title: '时间',
      render: (x) => <XTime model={x} />,
    },
    {
      title: '操作',
      width: 200,
      render: (text, record) => {
        return (
          <Button.Group size="small">
            <Button
              icon={<EditFilled />}
              type="primary"
              onClick={() => {
                _formData(record);
                _showForm(true);
              }}
            >
              编辑
            </Button>
            <Button
              icon={<DeleteFilled />}
              loading={loadingId == record.Id}
              type="primary"
              danger
              onClick={() => {
                deleteBrand(record);
              }}
            >
              删除
            </Button>
          </Button.Group>
        );
      },
    },
  ];

  useEffect(() => {
    queryList();
  }, []);

  return (
    <>
      <BrandForm
        show={showForm}
        hide={() => _showForm(false)}
        data={formData}
        ok={() => {
          _showForm(false);
          queryList();
        }}
      />
      <Card
        title="标签"
        size="small"
        extra={
          <Button
            type="primary"
            onClick={() => {
              _formData({});
              _showForm(true);
            }}
          >
            新增
          </Button>
        }
      >
        <Table
          size="small"
          rowKey={(x) => x.Id}
          loading={loading}
          columns={columns}
          dataSource={data || []}
          pagination={false}
        />
      </Card>
    </>
  );
};
