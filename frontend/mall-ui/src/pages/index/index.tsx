import XFooter from '@/components/footer';
import u from '@/utils';
import { Box, Container } from '@mui/material';
import { useEffect, useState } from 'react';
import { useModel } from 'umi';
import XAbout from './about';
import XBottom from './bottom';
import XGoods from './components/goods';
import XGoodsByCategorySeoName from './components/goodsByCategoryName';
import XHeader from './components/header';
import XLoading from './components/loading';
import XGuide from './guide';
import XNotice from './notice';
import XSlider from './slider';
import XTopic from './topic';

export default function IndexPage() {
  const appSettings = useModel('storeAppSetting');
  const [data, _data] = useState<any>({});
  const [blocks, _blocks] = useState({
    Blocks: '',
  });
  const [loading, _loading] = useState(false);
  const [loadingBlocks, _loadingBlocks] = useState(false);

  const queryHomePageData = () => {
    _loading(true);
    u.http.apiRequest
      .post('/mall/home/view')
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data.Data || {});
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  const queryBlocks = () => {
    _loadingBlocks(true);
    u.http.apiRequest
      .post('/mall/setting/home-blocks')
      .then((res) => {
        u.handleResponse(res, () => {
          _blocks(res.data.Data || {});
        });
      })
      .finally(() => {
        _loadingBlocks(false);
      });
  };

  useEffect(() => {
    queryHomePageData();
    queryBlocks();
    appSettings.queryMallSettings();
  }, []);

  useEffect(() => {
    var originColor = document.body.style.backgroundColor;
    document.body.style.backgroundColor = 'rgb(254,254,254)';
    return () => {
      document.body.style.backgroundColor = originColor;
    };
  }, []);

  const categories: any[] = data.CategoryAndGoods || [];

  return (
    <>
      <Container maxWidth="sm" disableGutters>
        <XHeader>
          <Box
            sx={{
              px: 0,
            }}
          >
            {loading && <XLoading />}
            <XNotice />
            <XSlider
              coupon={data.Coupons || []}
              promotion={data.Promotions || []}
              ok={() => {
                queryHomePageData();
              }}
            />
            <XAbout />
            <XGoods title="热销商品🔥" data={data.HotGoods} loading={loading} />
            <XGoods title="最近上新" data={data.NewGoods} loading={loading} />
            {categories.map((x, index) => (
              <Box sx={{}} key={index}>
                <XGoodsByCategorySeoName
                  category={x.Category || {}}
                  goods={x.Goods || []}
                  loading={loading}
                />
              </Box>
            ))}
            <XTopic data={blocks.Blocks} loading={loadingBlocks} />
            <XGuide />
            <XFooter />
            <XBottom />
          </Box>
        </XHeader>
      </Container>
    </>
  );
}
