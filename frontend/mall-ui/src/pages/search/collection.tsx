import XCombinationItem from '@/components/goods/combinationItem';
import u from '@/utils';
import { GoodsCombinationDto } from '@/utils/models';
import { Box, Typography } from '@mui/material';
import { Swiper } from 'antd-mobile';
import { useEffect, useState } from 'react';

export default function PinnedSubheaderList({ model }: { model: any }) {
  const [loading, _loading] = useState(false);
  const [data, _data] = useState<any[]>([]);

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

  const renderBox = (x: any) => {
    var items: any[] = x.Items || [];
    var combination: GoodsCombinationDto | null | undefined = items
      .map((x) => {
        var m: GoodsCombinationDto = x.GoodsSpecCombination;

        if (m != null) {
          m.Goods = m.Goods || x.Goods;
        }

        return m;
      })
      .find((x) => x != null);
    return (
      <Box
        sx={{ p: 1 }}
        onClick={() => {
          //
        }}
      >
        <Typography variant="subtitle2" gutterBottom component={'div'}>
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
      <Box sx={{ py: 2, backgroundColor: 'rgb(250,250,250)' }}>
        <Typography
          sx={{ px: 2 }}
          component="div"
          variant="overline"
          color="text.disabled"
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
