import XBacktotop from '@/components/backToTop';
import XEmpty from '@/components/empty';
import XItem from '@/components/goods/box';
import XDetailPreview from '@/components/goods/detail/detailPreview';
import XLoadMore from '@/components/infiniteScroller';
import u from '@/utils';
import http from '@/utils/http';
import { GoodsDto, PagedResponse, SearchGoodsInputDto } from '@/utils/models';
import { Masonry } from '@mui/lab';
import { Box } from '@mui/material';
import * as React from 'react';
import { history, useLocation } from 'umi';
import XSearchBox from './searchBox';
import utils from './utils';

const index = function (props: any) {
  const [detailId, _detailId] = React.useState(0);

  const [finalQuery, _finalQuery] = React.useState<SearchGoodsInputDto>({});
  const [itemList, _itemList] = React.useState<GoodsDto[]>([]);
  const [hasMore, _hasMore] = React.useState(false);
  const [options, _options] = React.useState<any>({});

  const [queryLoading, _queryLoading] = React.useState(false);
  const [loadingOption, _loadingOption] = React.useState(false);

  const querySearchOptions = (param: SearchGoodsInputDto) => {
    if (param.TagId || param.BrandId || param.CategoryId) {
    } else {
      console.log('没有搜索条件');
      return;
    }
    _loadingOption(true);
    http.apiRequest
      .post('/mall/search/search-options', {
        ...param,
      })
      .then((res) => {
        _options(res.data.Data || {});
      })
      .finally(() => {
        _loadingOption(false);
      });
  };

  React.useEffect(() => {
    querySearchOptions(finalQuery);
  }, [finalQuery.TagId, finalQuery.BrandId, finalQuery.CategoryId]);

  const queryGoods = (q: SearchGoodsInputDto) => {
    if (q.Page && q.Page >= 1) {
    } else {
      q.Page = 1;
    }

    if (queryLoading) {
      return;
    }

    _queryLoading(true);
    return new Promise<void>((resolve, reject) => {
      http.apiRequest
        .post('/mall/search/goods', {
          ...q,
        })
        .then((res: { data: PagedResponse<GoodsDto> }) => {
          var items = res.data.Items || [];
          _hasMore(!u.isEmpty(items));
          _itemList((x) => [...x, ...items]);
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

  const location = useLocation();

  const triggerQueryFromLocation = (location: any) => {
    var param = utils.buildQueryFromUrl(location.query);

    console.log('param from location', param);

    _itemList([]);
    _finalQuery(param);
    queryGoods(param);
  };

  React.useEffect(() => {
    triggerQueryFromLocation(location);
    var unlistener = history.listen((location) => {
      triggerQueryFromLocation(location);
    });

    return () => {
      console.log('leave goods list');
      unlistener && unlistener();
    };
  }, []);

  return (
    <>
      <XBacktotop {...props} />
      <XDetailPreview
        detailId={detailId}
        onClose={() => {
          _detailId(0);
        }}
      />
      <Box sx={{ mb: 2, px: 1 }}>
        <XSearchBox
          loadingOption={loadingOption}
          options={options}
          finalQuery={finalQuery}
          onSearch={(param: any) => {
            _itemList((x) => []);
            _finalQuery(param);
            queryGoods(param);
          }}
        />
      </Box>
      <XLoadMore
        loading={queryLoading}
        hasMore={hasMore}
        onLoad={async () => {
          const page = finalQuery.Page || 1;
          var param = {
            ...finalQuery,
            Page: page + 1,
          };
          console.log('more', finalQuery, param);
          _finalQuery(param);
          await queryGoods(param);
        }}
      >
        <Box sx={{ mb: 1 }}>
          {!queryLoading && u.isEmpty(itemList) && <XEmpty />}
          {u.isEmpty(itemList) || (
            <Masonry
              defaultHeight={300}
              defaultColumns={2}
              defaultSpacing={2}
              columns={{
                xs: 2,
                sm: 3,
                md: 4,
              }}
              spacing={{
                xs: 1,
                sm: 2,
                md: 2,
              }}
            >
              {u.map(itemList, (item, index) => (
                <Box
                  sx={{}}
                  component={'div'}
                  key={index}
                  onClick={() => {
                    item.Id && _detailId(item.Id);
                    //storeState();
                  }}
                >
                  <XItem data={{ model: item }} />
                </Box>
              ))}
            </Masonry>
          )}
        </Box>
      </XLoadMore>
    </>
  );
};

export default index;
