import XTime from '@/components/manage/time';
import u from '@/utils';
import http from '@/utils/http';
import { ReloadOutlined } from '@ant-design/icons';
import {
  Button,
  Card,
  message,
  Space,
  Table,
  Tooltip,
  Popover,
  Tag,
} from 'antd';
import { ColumnType } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import XPrice from './form/price';
import XStatus from './form/status';
import XStock from './form/stock';
import XSpecForm from './form/spec';
import XDetailModal from './detailModal';
import ImgPreview from '@/components/image/PreviewGroup';
import XSearchForm from './searchForm';
import XStore from './form/store';
import XQrcode from '@/components/qrcode';
import { GoodsCombinationDto } from '@/utils/models';

export default (props: any): React.ReactNode => {
  const [loading, _loading] = useState(true);
  const [data, _data] = useState({
    Items: [],
    TotalCount: 0,
  });
  const [query, _query] = useState({
    Page: 1,
  });

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/combination/query-paging', {
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

  const columns: ColumnType<any>[] = [
    {
      title: '名称',
      width: 150,
      fixed: 'left',
      render: (x) => (
        <>
          <div>
            <a href={`/store/goods/${x.Goods?.Id}`} target="blank">
              {`${x.Goods?.Name}`}
            </a>
          </div>
          <p>{x.Name}</p>
          {u.isEmpty(x.Sku) || (
            <Popover
              content={<XQrcode value={x.Sku || ''} height={50} />}
              title="SKU"
            >
              <p>
                <Tag>{`编号：${x.Sku}`}</Tag>
              </p>
            </Popover>
          )}
        </>
      ),
    },
    {
      title: '规格组合',
      render: (x) => (
        <XSpecForm
          model={x}
          ok={() => {
            queryList();
          }}
        />
      ),
    },
    {
      title: '价格',
      render: (x) => (
        <XPrice
          model={x}
          ok={() => {
            queryList();
          }}
        />
      ),
    },
    {
      title: '库存',
      render: (x) => (
        <XStock
          model={x}
          ok={() => {
            queryList();
          }}
        />
      ),
    },
    {
      title: '在售门店',
      render: (x) => (
        <XStore
          data={x}
          ok={() => {
            queryList();
          }}
        />
      ),
    },
    {
      title: '状态',
      render: (x) => (
        <XStatus
          model={x}
          ok={() => {
            queryList();
          }}
        />
      ),
    },
    {
      title: '图片',
      render: (record: GoodsCombinationDto) => {
        return (
          <ImgPreview
            data={(record.Goods?.XPictures || []).filter(
              (x) => x.CombinationId == record.Id,
            )}
          />
        );
      },
    },
    {
      title: '时间',
      render: (x) => <XTime model={x} />,
    },
    {
      title: '详情',
      render: (x) => {
        if (x.IsDeleted) {
          return <span>规格被删除</span>;
        }
        return <XDetailModal model={x} />;
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
          _data((x) => ({
            ...x,
            Items: [],
          }));
          _query(q);
        }}
      />
      <Card
        size="small"
        extra={
          <Space>
            <Tooltip title="刷新当前页面">
              <Button
                icon={<ReloadOutlined />}
                onClick={() => {
                  queryList();
                }}
              ></Button>
            </Tooltip>
          </Space>
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
              _data((x) => ({
                ...x,
                Items: [],
              }));
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
