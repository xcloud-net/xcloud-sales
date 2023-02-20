import u from '@/utils';
import http from '@/utils/http';
import { Alert, Box, Button, Container } from '@mui/material';
import { useEffect, useState } from 'react';
//import XItem from './item';
import XDetailPreview from '@/components/goods/detail/detailPreview';
import XItem from '@/components/goods/item';
import XLoadMore from '@/components/infiniteScroller';
import API from '@/services/storeApp';

export default function AlignItemsList() {
  const [data, setData] = useState<any>([]);
  const [query, _query] = useState<any>({
    Page: 1,
    initial: true,
  });
  const [hasMore, _hasMore] = useState(false);
  const [loading, _loading] = useState(false);

  const queryPage = (q: any) => {
    if (q.initial) {
      return;
    }

    _loading(true);
    return new Promise<void>((resolve, reject) => {
      http.apiRequest
        .post('/mall/favorites/pagingv1', {
          ...q,
        })
        .then((res) => {
          var items = res.data.Items || [];
          _hasMore(!u.isEmpty(items));
          setData((prev: any) => [...prev, ...items]);
          resolve();
        })
        .catch((e) => {
          reject(e);
        })
        .finally(() => {
          _loading(false);
        });
    });
  };

  const removeFavorites = (x: any) => {
    if (!confirm('确定删除？')) {
      return;
    }
    API.removeFavorite(x.GoodsId).then((res) => {
      u.handleResponse(res, () => {
        setData(u.filter(data, (d) => d.GoodsId != x.GoodsId));
      });
    });
  };

  useEffect(() => {
    var q = { ...query };
    q.initial = false;
    _query(q);
    queryPage(q);
  }, []);

  const [detailId, _detailId] = useState(0);
  return (
    <>
      <Container maxWidth="sm">
        <XDetailPreview
          detailId={detailId}
          onClose={() => {
            _detailId(0);
          }}
        />
        <XLoadMore
          loading={loading}
          hasMore={hasMore}
          onLoad={async () => {
            var q = { ...query };
            q.Page++;
            _query(q);
            await queryPage(q);
          }}
        >
          {u.map(data || [], (x, index) => {
            const { Goods } = x;
            return (
              <Box sx={{ my: 2 }} key={index}>
                {Goods == null && (
                  <Alert
                    color="info"
                    action={
                      <Button
                        onClick={() => {
                          removeFavorites(x);
                        }}
                      >
                        删除
                      </Button>
                    }
                  >
                    商品被下架或者删除
                  </Alert>
                )}
                {Goods != null && (
                  <Box
                    onClick={() => {
                      _detailId(Goods.Id);
                    }}
                  >
                    <XItem model={Goods} />
                  </Box>
                )}
              </Box>
            );
          })}
        </XLoadMore>
      </Container>
    </>
  );
}
