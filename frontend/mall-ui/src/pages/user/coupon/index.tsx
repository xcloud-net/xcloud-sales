import XUserCouponItem from '@/components/coupon/userCouponItem';
import XLoadMore from '@/components/infiniteScroller';
import u from '@/utils';
import http from '@/utils/http';
import * as React from 'react';
import { Box, Container } from '@mui/material';

export default function AlignItemsList() {
  const [items, _items] = React.useState<any[]>([]);
  const [hasMore, _hasMore] = React.useState(false);
  const [queryLoading, _queryLoading] = React.useState(false);
  const [query, _query] = React.useState({});

  //CategoryId
  const queryGoods = (q: any) => {
    if (q.Page && q.Page >= 1) {
    } else {
      q.Page = 1;
    }

    _queryLoading(true);
    return new Promise<void>((resolve, reject) => {
      http.apiRequest
        .post('mall/coupon/user-coupon-paging', {
          ...q,
        })
        .then((res) => {
          var data = res.data.Items || [];
          _hasMore(!u.isEmpty(data));
          _items((x) => [...x, ...data]);
          resolve();
        })
        .catch((e) => reject(e))
        .finally(() => {
          _queryLoading(false);
        });
    });
  };

  React.useEffect(() => {
    queryGoods({ ...query });
  }, []);

  return (
    <>
      <Container maxWidth="sm" disableGutters>
        <Box sx={{ px: 1 }}>
          <XLoadMore
            loading={queryLoading}
            hasMore={hasMore}
            onLoad={async () => {
              var p = {
                Page: 1,
                ...query,
              };
              p.Page++;
              _query(p);
              await queryGoods(p);
            }}
          >
            {u.isEmpty(items) ||
              u.map(items, (x, index) => {
                return (
                  <Box
                    key={index}
                    sx={{ mb: 2 }}
                    onClick={() => {
                      //
                    }}
                  >
                    <XUserCouponItem model={x} />
                  </Box>
                );
              })}
          </XLoadMore>
        </Box>
      </Container>
    </>
  );
}
