import u from '@/utils';
import http from '@/utils/http';
import { PlusOutlined, ReloadOutlined } from '@ant-design/icons';
import { Alert, Button, Card, message, Space, Table, Tooltip } from 'antd';
import { ColumnType } from 'antd/es/table';
import React, { useEffect, useState } from 'react';
import XCombinationList from './combinationList';
import XEdit from './form';
import ImgPreview from '@/components/image/PreviewGroup';
import XSearchForm from './searchForm';
import XStatus from './status';
import XTime from '@/components/manage/time';
import { GoodsDto } from '@/utils/models';

export default (props: any): React.ReactNode => {
  const [loading, _loading] = useState(true);
  const [data, _data] = useState({
    Items: [],
    TotalCount: 0,
  });
  const [query, _query] = useState({
    Page: 1,
  });

  const [editShow, _editShow] = useState(false);
  const [editId, _editId] = useState(0);

  const queryList = () => {
    _loading(true);
    http.apiRequest
      .post('/mall-admin/goods/paging', {
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
      render: (x: GoodsDto) => (
        <>
          <div>
            <a href={`/store/goods/${x.Id}`} target="blank">
              {x.Name}
            </a>
          </div>
          <p>{u.map(x.GoodsSpecCombinations || [], (d) => d.Name).join(',')}</p>
        </>
      ),
    },
    {
      title: '基础信息',
      render: (record: GoodsDto) => (
        <>
          <div>品牌:{record.Brand?.Name || '--'}</div>
          <div>分类:{record.Category?.Name || '--'}</div>
        </>
      ),
    },
    {
      title: '上架',
      render: (record) => {
        return (
          <XStatus
            model={record}
            ok={() => {
              queryList();
            }}
          />
        );
      },
    },
    {
      title: '图片',
      render: (record: GoodsDto) => {
        return <ImgPreview data={record.XPictures || []} />;
      },
    },
    {
      title: '时间',
      render: (x) => <XTime model={x} />,
    },
    {
      title: '操作',
      width: 150,
      fixed: 'right',
      render: (record) => {
        if (record.IsDeleted) {
          return (
            <>
              <span>商品被删除</span>
            </>
          );
        }
        return (
          <Button.Group size="small">
            <Button
              type="primary"
              onClick={() => {
                _editId(record.Id);
                _editShow(true);
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
      <XEdit
        goodsId={editId}
        show={editShow}
        hide={() => {
          _editShow(false);
          _editId(-1);
        }}
        ok={() => {}}
      />
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
            <Button
              type="primary"
              icon={<PlusOutlined />}
              onClick={() => {
                _editId(0);
                _editShow(true);
              }}
            >
              新增
            </Button>
          </Space>
        }
      >
        <Table
          size="small"
          rowKey={(x) => x.Id}
          loading={loading}
          columns={columns}
          dataSource={data.Items || []}
          expandable={{
            expandedRowRender: (x) => {
              return u.isEmpty(x.GoodsSpecCombinations) ? (
                <Alert message="未设置规格"></Alert>
              ) : (
                <XCombinationList items={x.GoodsSpecCombinations} />
              );
            },
          }}
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
