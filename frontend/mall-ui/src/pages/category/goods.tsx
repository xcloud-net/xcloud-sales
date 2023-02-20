import XDetailPreview from '@/components/goods/detail/detailPreview';
import XItem from '@/components/goods/item/base';
import XLoadMore from '@/components/infiniteScroller';
import u from '@/utils';
import http from '@/utils/http';
import { GoodsDto, SearchGoodsInputDto } from '@/utils/models';
import Box from '@mui/material/Box';
import * as React from 'react';

export default function VerticalTabs(props: any) {
  const { categoryId } = props;

  const [items, _items] = React.useState<GoodsDto[]>([]);
  const [hasMore, _hasMore] = React.useState(false);
  const [queryLoading, _queryLoading] = React.useState(false);

  const [query, _query] = React.useState<SearchGoodsInputDto>({
    Page: 1,
    CategoryId: -1,
  });

  React.useEffect(() => {
    _items((x) => []);
    var queryParam = {
      Page: 1,
      CategoryId: categoryId,
    };
    _query(queryParam);
    queryGoods(queryParam);
  }, [categoryId]);

  //CategoryId
  const queryGoods = (q: SearchGoodsInputDto) => {
    if (!q.CategoryId || q.CategoryId <= 0) {
      return;
    }
    _queryLoading(true);
    return new Promise<void>((resolve, reject) => {
      http.apiRequest
        .post('mall/search/goods', {
          ...q,
        })
        .then((res) => {
          var data = res.data.Items || [];
          _hasMore(!u.isEmpty(data));
          _items((x) => [...x, ...data]);
          resolve();
        })
        .catch((e) => {
          reject(e);
        })
        .finally(() => {
          _queryLoading(false);
        });
    });
  };

  const [detailId, _detailId] = React.useState(0);
  return (
    <>
      <XDetailPreview
        detailId={detailId}
        onClose={() => {
          _detailId(0);
        }}
      />
      <Box sx={{}}>
        <XLoadMore
          loading={queryLoading}
          hasMore={hasMore}
          onLoad={async () => {
            const page = query.Page || 1;
            const p = {
              ...query,
              Page: page + 1,
            };
            _query(p);
            await queryGoods(p);
          }}
        >
          {u.map(items || [], (x, index) => {
            return (
              <Box
                key={index}
                sx={{
                  mb: 2,
                  cursor: 'pointer',
                  '&:hover': {
                    border: (theme) =>
                      `1px solid ${theme.palette.primary.main}`,
                  },
                }}
                onClick={() => {
                  x.Id && _detailId(x.Id);
                }}
              >
                <XItem model={x} count={2} />
              </Box>
            );
          })}
        </XLoadMore>
      </Box>
    </>
  );
}
