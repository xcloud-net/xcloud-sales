import { useEffect, useState } from 'react';
import { AfterSaleDto, AfterSalesCommentDto, PagedResponse } from '@/utils/models';
import u from '@/utils';
import XItem from './item';
import XForm from './form';
import { Box, Button } from '@mui/material';

export default ({ model }: { model: AfterSaleDto }) => {

  const [loading, _loading] = useState(false);
  const [data, _data] = useState<AfterSalesCommentDto[]>([]);
  const [query, _query] = useState({
    Page: 1,
  });
  const [hasMore, _hasMore] = useState(true);

  const queryComment = (model: AfterSaleDto, q: any) => {
    if (!hasMore) {
      return;
    }
    if (!model || u.isEmpty(model.Id)) {
      return;
    }
    _loading(true);
    u.http.apiRequest.post<PagedResponse<AfterSalesCommentDto>>('/mall/aftersale/comment-paging', {
      ...q,
      PageSize: 10,
      AfterSalesId: model.Id,
    }).then(res => {
      u.handleResponse(res, () => {
        const items = (res.data.Items || []);
        _data(x => [...x, ...items]);
        _hasMore(items.length > 0);
      });
    }).finally(() => {
      _loading(false);
    });
  };

  const renderLoadMore = () => {
    return hasMore && <Box sx={{}}>
      <Button variant={'text'} onClick={() => {
        const q = { ...query, Page: query.Page + 1 };
        _query(q);
        queryComment(model, q);
      }}>加载更多</Button>
    </Box>;
  };

  const renderItemList = () => {
    return <Box sx={{}}>
      {sortedData.map((x, i) => <div key={i}><XItem model={x} /></div>)}
    </Box>;
  };

  const parseTime = (time?: string) => {
    try {
      if (time) {
        return u.dayjs(time).unix();
      }
    } catch (e) {
      console.log(e);
    }
    return u.dayjs().unix();
  };

  const sortedData = u.sortBy(data, x => parseTime(x.CreationTime));

  useEffect(() => {
    if (model && model.Id) {
      _data(x => []);
      queryComment(model, query);
    }
  }, [model]);

  return <>
    {loading && <div>loading...</div>}
    {loading || <Box sx={{}}>
      {renderLoadMore()}
      {renderItemList()}
    </Box>}
    <Box sx={{ my: 1 }}>
      <XForm model={model} />
    </Box>
  </>;
};
