import XBackToTop from '@/components/backToTop';
import XDetailDialog from '@/components/detailDialog';
import XLoadMore from '@/components/infiniteScroller';
import LinearProgress from '@/components/loading/linear';
import u from '@/utils';
import http from '@/utils/http';
import { OrderDto } from '@/utils/models';
import utils from '@/utils/order';
import { Badge, Box, Container } from '@mui/material';
import { Tabs } from 'antd-mobile';
import * as React from 'react';
import XDetail from '../detail';
import XEmpty from './empty';
import XItem from './item';

export default function AlignItemsList(props: any) {
  const [loading, _loading] = React.useState(false);
  const [items, _items] = React.useState<OrderDto[]>([]);
  const [hasMore, _hasMore] = React.useState(false);

  const [finalQuery, _finalQuery] = React.useState<any>({});

  const [pendingAftersaleCount, _pendingAftersaleCount] = React.useState(0);

  const [selectedTab, _selectedTab] = React.useState('pending');
  const [detailId, _detailId] = React.useState<string | null | undefined>('');

  const queryPendingAftersaleCount = () => {
    u.http.apiRequest.post('/mall/aftersale/pending-count').then((res) => {
      u.handleResponse(res, () => {
        _pendingAftersaleCount(res.data.Data || 0);
      });
    });
  };

  const queryOrders = (queryFilter: any) => {
    if (queryFilter.Page && queryFilter.Page >= 1) {
    } else {
      queryFilter.Page = 1;
    }

    _loading(true);
    return new Promise<void>((resolve, reject) => {
      http.apiRequest
        .post('/mall/order/paging', {
          ...queryFilter,
        })
        .then((res) => {
          if (res.data.Error) {
            alert(res.data.Error.Message);
          } else {
            var data = res.data.Items || [];
            _items((x) => [...x, ...data]);
            _hasMore(!u.isEmpty(data));
          }
          resolve();
        })
        .catch((e) => reject(e))
        .finally(() => {
          _loading(false);
        });
    });
  };

  const tabs = [
    {
      key: 'pending',
      name: '进行中',
      filter: {
        Status: [
          utils.OrderStatus.None,
          utils.OrderStatus.Pending,
          utils.OrderStatus.Processing,
        ],
        IsAfterSales: false,
      },
    },
    {
      key: 'delivering',
      name: '配送中',
      filter: {
        Status: [utils.OrderStatus.Delivering],
        IsAfterSales: false,
      },
    },
    {
      key: 'complete',
      name: '完成',
      filter: {
        Status: [utils.OrderStatus.Complete],
        IsAfterSales: false,
      },
    },
    {
      key: 'all',
      name: '全部',
      filter: {
        Status: null,
        IsAfterSales: false,
      },
    },
    {
      key: 'aftersale',
      name: '售后中',
      count: pendingAftersaleCount,
      filter: {
        Status: null,
        IsAfterSales: true,
      },
    },
  ];

  React.useEffect(() => {
    queryPendingAftersaleCount();
  }, []);

  React.useEffect(() => {
    var selectedTabData = tabs.find((x) => x.key == selectedTab);
    if (selectedTabData) {
      _items((x) => []);
      var p = {
        ...finalQuery,
        ...selectedTabData.filter,
        Page: 1,
      };
      _finalQuery(p);
      queryOrders(p);
    }
  }, [selectedTab]);

  const renderTab = () => {
    return (
      <Box sx={{ width: '100%', bgcolor: 'background.paper' }}>
        <Tabs
          activeKey={selectedTab}
          onChange={(e) => {
            _selectedTab(e);
          }}
        >
          {u.map(tabs, (x, index) => (
            <Tabs.Tab
              key={x.key}
              title={
                <Badge
                  badgeContent={x.count}
                  color="error"
                  variant="dot"
                  invisible={!(x.count && x.count > 0)}
                >
                  <span>{x.name}</span>
                </Badge>
              }
            />
          ))}
        </Tabs>
      </Box>
    );
  };

  const renderItemList = () => {
    return (
      <Box sx={{}}>
        {u.map(items, (x, index) => (
          <Box
            sx={{ my: 2 }}
            onClick={() => {
              _detailId(x.Id);
            }}
            key={index}
          >
            <XItem model={x} />
          </Box>
        ))}
      </Box>
    );
  };

  return (
    <>
      <Container disableGutters maxWidth="sm">
        <XBackToTop {...props} />

        <XDetailDialog
          open={!u.isEmpty(detailId)}
          onClose={() => {
            _detailId('');
          }}
        >
          <XDetail
            orderId={detailId || ''}
            ok={() => {
              //trigger refresh list
              queryPendingAftersaleCount();
            }}
          />
        </XDetailDialog>
        <XLoadMore
          loading={loading}
          hasMore={hasMore}
          onLoad={async () => {
            var param = {
              Page: 1,
              ...finalQuery,
            };
            param.Page++;
            _finalQuery(param);
            await queryOrders(param);
          }}
        >
          {loading && <LinearProgress />}
          {renderTab()}

          {u.isEmpty(items) && <XEmpty />}
          {u.isEmpty(items) || renderItemList()}
        </XLoadMore>
      </Container>
    </>
  );
}
