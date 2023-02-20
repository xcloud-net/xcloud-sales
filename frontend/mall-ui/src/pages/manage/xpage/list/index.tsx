import XTime from '@/components/manage/time';
import http from '@/utils/http';
import { EditFilled } from '@ant-design/icons';
import { Button, Card, message, Table } from 'antd';
import { ColumnProps } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import XDesign from '../editor';
import BrandForm from './form';
import XSearchForm from './searchForm';
import XStatus from './status';

export default (props: any): React.ReactNode => {
  const [loading, _loading] = useState(true);
  const [data, _data] = useState({
    Items: [],
    TotalCount: 0,
  });
  const [query, _query] = useState({
    Page: 1,
  });

  const [showForm, _showForm] = useState(false);
  const [formData, _formData] = useState({});

  const [design, _design] = useState(null);

  const [loadingId, _loadingId] = useState(0);

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/pages/paging', {
        ...query,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          _data(res.data || {});
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const columns: ColumnProps<any>[] = [
    {
      title: '访问地址',
      render: (x) => x.SeoName || '--',
    },
    {
      title: '名称',
      render: (x) => (
        <a href={`/store/pages/${x.Id}`} target={'_blank'}>
          {x.Title || '--'}
        </a>
      ),
    },
    {
      title: '描述',
      render: (x) => x.Description || '--',
    },
    {
      title: '关键词',
      render: (x) => x.MetaKeywords || '--',
    },
    {
      title: '点击量',
      render: (x) => x.ReadCount,
    },
    {
      title: '状态',
      render: (x) => (
        <XStatus
          data={x}
          ok={() => {
            queryList();
          }}
        />
      ),
    },
    {
      title: '发布时间',
      render: (x) => x.PublishedTime,
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
              icon={<EditFilled />}
              type="dashed"
              onClick={() => {
                _design(record);
              }}
            >
              设计页面
            </Button>
          </Button.Group>
        );
      },
    },
  ];

  useEffect(() => {
    queryList();
  }, [query]);

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
      <XDesign
        show={design != null}
        hide={() => _design(null)}
        data={design}
        ok={() => {
          _design(null);
          queryList();
        }}
      />
      <XSearchForm
        query={query}
        onSearch={(q: any) => {
          _query(q);
        }}
      />
      <Card
        title="页面"
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
          dataSource={data.Items || []}
          pagination={{
            showSizeChanger: false,
            pageSize: 20,
            current: query.Page,
            total: data.TotalCount,
            onChange: (e) => {
              _query({
                ...query,
                Page: e,
              });
            },
          }}
        />
      </Card>
    </>
  );
};
