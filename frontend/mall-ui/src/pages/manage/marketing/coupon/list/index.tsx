import XTime from '@/components/manage/time';
import http from '@/utils/http';
import { EditFilled } from '@ant-design/icons';
import { Button, Card, message, Table } from 'antd';
import { ColumnProps } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import BrandForm from './form';
import XSearchForm from './searchForm';
import XStatus from './status';
import { CouponDto } from '@/utils/models';

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
      .post('/mall-admin/coupon/paging', {
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
      render: (x: CouponDto) => x.Title || '--',
    },
    {
      title: '金额',
      render: (x: CouponDto) => x.Value || 0,
    },
    {
      title: '数量',
      render: (x: CouponDto) => x.Amount || 0,
    },
    {
      title: '限制数量',
      render: (x: CouponDto) => x.IsAmountLimit,
    },
    {
      title: '领取限制',
      render: (x: CouponDto) => x.AccountIssuedLimitCount,
    },
    {
      title: '领取有效期',
      render: (x: CouponDto) => x.ExpiredDaysFromIssue,
    },
    {
      title: '开始时间',
      render: (x: CouponDto) => x.StartTime,
    },
    {
      title: '结束时间',
      render: (x: CouponDto) => x.EndTime,
    },
    {
      title: '领取数量',
      render: (x: CouponDto) => x.IssuedAmount || 0,
    },
    {
      title: '使用数量',
      render: (x: CouponDto) => x.UsedAmount || 0,
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
      render: (record) => {
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
        title="优惠券"
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
