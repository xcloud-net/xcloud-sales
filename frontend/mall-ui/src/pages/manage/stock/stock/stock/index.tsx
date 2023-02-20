import XTime from '@/components/manage/time';
import u from '@/utils';
import http from '@/utils/http';
import { PagedResponse, StockDto } from '@/utils/models';
import { CheckOutlined, DeleteFilled } from '@ant-design/icons';
import { Button, Card, Table, Tag, message } from 'antd';
import { ColumnProps } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import XItems from './items';
import XSearchForm from './searchForm';
import BrandForm from './xform';

export default (props: any): React.ReactNode => {
  const [loading, _loading] = useState(true);
  const [data, _data] = useState<PagedResponse<StockDto>>({
    Items: [],
    TotalCount: 0,
  });
  const [query, _query] = useState({
    Page: 1,
  });

  const [showForm, _showForm] = useState(false);
  const [formData, _formData] = useState({});

  const [loadingId, _loadingId] = useState('');

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/stock/paging', {
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

  const approveStock = (row: StockDto) => {
    if (row.Approved) {
      return;
    }
    if (!confirm('确定批准采购单？')) {
      return;
    }
    _loadingId(row.Id || '');
    http.apiRequest
      .post('/mall-admin/stock/approve', {
        Id: row.Id,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          message.success('已经批准采购');
          queryList();
        });
      })
      .finally(() => {
        _loadingId('');
      });
  };

  const deleteBrand = (row: any) => {
    if (!confirm('删除入库单？')) {
      return;
    }
    _loadingId(row.Id);
    http.apiRequest
      .post('/mall-admin/stock/delete-unapproved-stock', {
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
        _loadingId('');
      });
  };

  const columns: ColumnProps<any>[] = [
    {
      title: '批号',
      render: (x: StockDto) => <b>{`${x.No || '--'}`}</b>,
    },
    {
      title: '备注信息',
      render: (x) => x.Remark || '--',
    },
    {
      title: '过期时间',
      render: (x: StockDto) => {
        if (u.isEmpty(x.ExpirationTime)) {
          return <span>--</span>;
        }
        var expired = u.dayjs(x.ExpirationTime).isBefore(u.dayjs());
        if (expired) {
          return (
            <span style={{ color: 'red' }}>
              已过期：{x.ExpirationTime || '--'}
            </span>
          );
        }
        return <span>{x.ExpirationTime || '--'}</span>;
      },
    },
    {
      title: '状态',
      render: (x: StockDto) => {
        return x.Approved ? (
          <Tag color="green">已批准</Tag>
        ) : (
          <Tag color="yellow">待审核</Tag>
        );
      },
    },
    {
      title: '时间',
      render: (x) => <XTime model={x} />,
    },
    {
      title: '操作',
      width: 200,
      render: (record: StockDto) => {
        return (
          <Button.Group size="small">
            {record.Approved || (
              <Button
                icon={<CheckOutlined />}
                loading={loadingId == record.Id}
                type="primary"
                onClick={() => {
                  approveStock(record);
                }}
              >
                审核
              </Button>
            )}
            {record.Approved || (
              <Button
                icon={<DeleteFilled />}
                loading={loadingId == record.Id}
                danger
                onClick={() => {
                  deleteBrand(record);
                }}
              >
                删除
              </Button>
            )}
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
        title="采购单"
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
          expandable={{
            expandedRowRender: (e: StockDto) => <XItems data={e.Items || []} />,
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
