import XDetailPreview from '@/components/goods/detail/detailPreview';
import XGoods from '@/components/goods/item';
import u from '@/utils';
import { IPageGoodsCollectionItem } from '@/utils/models';
import { Box, Skeleton } from '@mui/material';
import { useEffect, useState } from 'react';

export default function IndexPage(props: { data: IPageGoodsCollectionItem }) {
  const { data } = props;

  if (!data || data.type != 'goods-collection') {
    return null;
  }

  const { goodsIds } = data;

  const [detailId, _detailId] = useState(0);
  const [loading, _loading] = useState(false);
  const [goods, _goods] = useState<any>([]);

  const queryGoods = () => {
    if (goodsIds == null || u.isEmpty(goodsIds)) {
      return;
    }
    _loading(true);
    u.http.apiRequest
      .post('/mall/goods/multiple-by-ids', [...goodsIds])
      .then((res) => {
        u.handleResponse(res, () => {
          _goods(res.data.Data || []);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryGoods();
  }, [goodsIds]);

  return (
    <>
      <XDetailPreview
        detailId={detailId}
        onClose={() => {
          _detailId(0);
        }}
      />
      {loading && (
        <Box sx={{}}>
          <Skeleton />
          <Skeleton animation="wave" />
          <Skeleton animation={false} />
        </Box>
      )}
      {loading || (
        <>
          <Box sx={{ px: 1 }}>
            {u.map(goods, (x, index) => {
              return (
                <Box
                  key={index}
                  sx={{ mb: 1 }}
                  onClick={() => {
                    _detailId(x.Id);
                  }}
                >
                  <XGoods model={x} />
                </Box>
              );
            })}
          </Box>
        </>
      )}
    </>
  );
}
