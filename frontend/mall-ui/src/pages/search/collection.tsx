import XCombinationItem from '@/components/goods/combinationItem';
import u from '@/utils';
import { GoodsCollectionDto, GoodsCombinationDto } from '@/utils/models';
import { Box, Typography } from '@mui/material';
import { Swiper } from 'antd-mobile';
import { useEffect, useState } from 'react';
import XDetailPreview from '@/components/collection/detail/detailPreview';

export default function PinnedSubheaderList({ model }: { model: any }) {
  const [loading, _loading] = useState(false);
  const [data, _data] = useState<GoodsCollectionDto[]>([]);
  const [detailId, _detailId] = useState<string>('');

  const queryCollection = () => {
    _loading(true);
    u.http.apiRequest
      .post('/mall/collection/top')
      .then((res) => {
        _data(res.data.Data || []);
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryCollection();
  }, []);

  const renderBox = (x: GoodsCollectionDto) => {
    var items = x.Items || [];
    var combination: GoodsCombinationDto | null | undefined = items
      .map((x) => {
        return x.GoodsSpecCombination;
      })
      .find((x) => x != null);
    return (
      <Box
        sx={{ p: 1 }}
        onClick={() => {
          _detailId(x.Id || '');
        }}
      >
        <Typography variant='subtitle2' gutterBottom component={'div'}>
          {x.Name || '--'}
        </Typography>
        {combination == null || <XCombinationItem model={combination} />}
      </Box>
    );
  };

  if (loading) {
    //
  }

  if (u.isEmpty(data)) {
    return null;
  }

  return (
    <>
      <XDetailPreview detailId={detailId} onClose={() => {
        _detailId('');
      }} />
      <Box sx={{ py: 2, backgroundColor: 'rgb(250,250,250)' }}>
        <Typography
          sx={{ px: 2 }}
          component='div'
          variant='overline'
          color='text.disabled'
          gutterBottom
        >
          商品集
        </Typography>
        <Box sx={{ mt: 1 }}>
          <Swiper
            trackOffset={10}
            slideSize={80}
            style={{
              '--border-radius': '8px',
            }}
            defaultIndex={0}
            indicator={() => null}
          >
            {data.map((x, index) => (
              <Swiper.Item key={`collection-${index}`}>
                {renderBox(x)}
              </Swiper.Item>
            ))}
          </Swiper>
        </Box>
      </Box>
    </>
  );
}
