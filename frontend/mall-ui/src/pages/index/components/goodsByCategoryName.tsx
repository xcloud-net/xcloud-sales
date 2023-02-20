import XGoodsCard from '@/components/goods/box';
import XDetailPreview from '@/components/goods/detail/detailPreview';
import u from '@/utils';
import { Box, Button, Typography } from '@mui/material';
import { Swiper } from 'antd-mobile';
import { useState } from 'react';
import { history } from 'umi';
import XEmpty from './empty';
import XLoading from './loading';

export default (props: { category: any; goods: any; loading: boolean }) => {
  const { category, goods, loading } = props;
  const [detailId, _detailId] = useState(0);

  const renderSlider = () => {
    return (
      <>
        {u.isEmpty(goods) && <XEmpty />}
        {u.isEmpty(goods) || (
          <Swiper
            trackOffset={10}
            slideSize={40}
            defaultIndex={0}
            style={{
              '--track-padding': ' 0 0 16px',
            }}
          >
            {u.map(goods, (x: any, index) => (
              <Swiper.Item key={index}>
                <Box
                  onClick={() => {
                    _detailId(x.Id);
                  }}
                  sx={{ py: 1, px: 1 }}
                >
                  <XGoodsCard data={{ model: x, count: 1, lazy: false }} />
                </Box>
              </Swiper.Item>
            ))}
          </Swiper>
        )}
      </>
    );
  };

  if (loading) {
    return <XLoading />;
  }
  return (
    <>
      <XDetailPreview
        detailId={detailId}
        onClose={() => {
          _detailId(0);
        }}
      />
      <Box
        sx={{
          backgroundColor: '#fbfbfb',
          py: 2,
        }}
      >
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'space-between',
            px: 1,
          }}
        >
          <Typography variant="h3" gutterBottom sx={{}}>
            {category.Name || '--'}
          </Typography>
          <Button
            onClick={() => {
              category &&
                category.Id > 0 &&
                history.push({
                  pathname: '/category',
                  query: { cat: category.Id },
                });
            }}
          >
            更多
          </Button>
        </Box>
        <Box sx={{}}>{renderSlider()}</Box>
      </Box>
    </>
  );
};
