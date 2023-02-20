import XTime from '@/components/manage/time';
import http from '@/utils/http';
import { PromotionDto } from '@/utils/models';
import { EditFilled } from '@ant-design/icons';
import { Button, Card, message, Table } from 'antd';
import { ColumnProps } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import BrandForm from './form';
import XRules from './rules';
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

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/promotion/paging', {
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
      title: '名称',
      render: (x: PromotionDto) => x.Name || '--',
    },
    {
      title: '描述',
      render: (x: PromotionDto) => x.Description || '--',
    },
    {
      title: '规则',
      render: (x: PromotionDto) => <span>展开查看</span>,
    },
    {
      title: '开始时间',
      render: (x: PromotionDto) => x.StartTime,
    },
    {
      title: '结束时间',
      render: (x: PromotionDto) => x.EndTime,
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
      <XSearchForm
        query={query}
        onSearch={(q: any) => {
          _query(q);
        }}
      />
      <Card
        title="促销活动"
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
          expandable={{
            expandedRowRender: (x) => (
              <XRules
                model={x}
                ok={() => {
                  queryList();
                }}
              />
            ),
          }}
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
