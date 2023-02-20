import http from '@/utils/http';
import { PagedResponse, StockItemDto } from '@/utils/models';
import { Card, Progress, Table, Tag, Tooltip, message } from 'antd';
import { ColumnProps } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import XSearchForm from './searchForm';

export default (props: any): React.ReactNode => {
  const [loading, _loading] = useState(true);
  const [data, _data] = useState<PagedResponse<StockItemDto>>({
    Items: [],
    TotalCount: 0,
  });
  const [query, _query] = useState({
    Page: 1,
  });

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/stock/item-paging', {
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
      title: '商品',
      render: (x: StockItemDto) => {
        return (
          <span>{`${x.Goods?.Name || '--'}/${
            x.Combination?.Name || '--'
          }`}</span>
        );
      },
    },
    {
      title: '仓库',
      render: (x: StockItemDto) => {
        return <span>{x.WarehouseStock?.Warehouse?.Name || '--'}</span>;
      },
    },
    {
      title: '供货商',
      render: (x: StockItemDto) => {
        return <span>{x.WarehouseStock?.Supplier?.Name || '--'}</span>;
      },
    },
    {
      title: '采购单价',
      render: (x: StockItemDto) => {
        return <span>{x.Price}</span>;
      },
    },
    {
      title: '采购数量',
      render: (x: StockItemDto) => {
        return <span>{x.Quantity}</span>;
      },
    },
    {
      title: '库存结余',
      render: (x: StockItemDto) => {
        if (x.RuningOut) {
          return <Tag color="red">已无库存</Tag>;
        }
        if (x.Quantity && x.Quantity > 0) {
        } else {
          return <span>采购数量异常</span>;
        }
        const surplus = (x.Quantity || 0) - (x.DeductQuantity || 0);
        var percent = (surplus / (x.Quantity || 0)) * 100;
        if (percent > 100) {
          return <span>计算错误</span>;
        }
        return (
          <Tooltip title={`剩余${surplus}`}>
            <Progress percent={percent} status="active" />
          </Tooltip>
        );
      },
    },
  ];

  useEffect(() => {
    queryList();
  }, [query]);

  return (
    <>
      <XSearchForm
        query={query}
        onSearch={(q: any) => {
          _query(q);
        }}
      />
      <Card title="库存明细" size="small">
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
